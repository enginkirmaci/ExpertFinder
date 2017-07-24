using Common.Entities;
using Common.Entities.Enums;
using Common.Web.IO;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Manage;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TransactionResult = Common.Entities.TransactionResult;

namespace ExpertFinder.Application
{
    public class UserEngine : IUserEngine
    {
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IHostingEnvironment _environment;
        private readonly INotificationService _notificationService;
        private readonly ICategoryServices _categoryServices;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly teklifcepteDBContext _dbContext;

        public UserEngine(
            IHttpContextAccessor httpContextAccessor,
            IHostingEnvironment environment,
            INotificationService notificationService,
            ICategoryServices categoryServices,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            teklifcepteDBContext dbContext)
        {
            _httpContextAccessor = httpContextAccessor;
            _environment = environment;
            _notificationService = notificationService;
            _categoryServices = categoryServices;
            _userManager = userManager;
            _signInManager = signInManager;
            _dbContext = dbContext;
        }

        public void ClearUserSession(string UserId)
        {
            _session.Remove("LOGGED_USER" + UserId);
        }

        async public Task<User> CurrentUser(string UserId)
        {
            User user = null;

            user = _session.GetObjectFromJson<User>("LOGGED_USER" + UserId);
            if (user == null)
            {
                user = await _dbContext.Users
                    .Include(i => i.District)
                    .Include(i => i.Province)
                    .Include(i => i.UserCategoryRelation).ThenInclude(i => i.Category)
                    .FirstOrDefaultAsync(i => i.Id == UserId);

                if (user != null && (!user.LockoutEnabled || user.TwoFactorEnabled))
                    _session.SetObjectAsJson("LOGGED_USER" + UserId, user);
                else
                    return null;
            }

            return user;
        }

        public void ClearUserToken(string UserId)
        {
            _session.Remove("LOGGED_USER_TOKEN" + UserId);
        }

        public int CurrentUserToken(string UserId, bool disableCache = false)
        {
            var TokenCount = _session.GetObjectFromJson<int?>("LOGGED_USER_TOKEN" + UserId);
            if (TokenCount == null || disableCache)
            {
                TokenCount =
                    _dbContext.BoughtHistory.Where(i => i.UserId == UserId && i.IsPaid).Select(i => i.Campain.TokenCount).Where(i => i.HasValue).AsEnumerable().Sum(i => i.Value)
                    - _dbContext.SpentHistory.Where(i => i.UserId == UserId && i.TokenSpent.HasValue).AsEnumerable().Sum(i => i.TokenSpent.Value);

                if (TokenCount != null)
                    _session.SetObjectAsJson("LOGGED_USER_TOKEN" + UserId, TokenCount);
            }

            return TokenCount.Value;
        }

        async public Task<User> GetUser(string slugUrl)
        {
            var user = await _dbContext.Users
                  .Include(i => i.District)
                  .Include(i => i.Province)
                  .Include(i => i.UserCategoryRelation).ThenInclude(i => i.Category)
                  .FirstOrDefaultAsync(i => i.SlugUrl == slugUrl);

            return user;
        }

        public IEnumerable<UserExperienceImages> GetGallery(string userID)
        {
            var gallery = _dbContext.UserExperienceImages
                .Where(i => i.UserId == userID)
                .ToList();

            return gallery;
        }

        public TransactionResult RemoveGalleryItem(string userID, long itemId)
        {
            try
            {
                var item = _dbContext.UserExperienceImages.FirstOrDefault(i => i.Id == itemId && i.UserId == userID);

                if (item != null)
                {
                    var upload = new FormUpload(_environment.WebRootPath);
                    upload.RemoveFile(item.Url, "profilegallery");

                    _dbContext.UserExperienceImages.Remove(item);
                    _dbContext.SaveChanges();
                }

                return new TransactionResult(type: TransactionType.Success);
            }
            catch
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public void AfterRegistration(string userID, string notificationDescription = null, IEnumerable<string> SelectedUserCategories = null)
        {
            try
            {
                var freeToken = _dbContext.Campain.FirstOrDefault(i => i.SlugUrl == "ilk-uyelik");
                if (freeToken != null)
                {
                    var bought = new BoughtHistory()
                    {
                        Id = Guid.NewGuid(),
                        UserId = userID,
                        CampainId = freeToken.Id,
                        CreatedDate = DateTime.Now,
                        IsPaid = true
                    };
                    _dbContext.BoughtHistory.Add(bought);

                    if (!string.IsNullOrEmpty(notificationDescription))
                        _notificationService.AddNotification(userID, NotificationTypes.RegistrationExternalProvider, param: notificationDescription);
                    else
                        _notificationService.AddNotification(userID, NotificationTypes.Registration);
                }

                if (SelectedUserCategories != null && SelectedUserCategories.Count() > 0)
                {
                    var categories = _categoryServices.GetCategories();
                    foreach (var relation in SelectedUserCategories)
                    {
                        var category = categories.FirstOrDefault(i => i.SlugUrl == relation);

                        if (category != null)
                            _dbContext.UserCategoryRelation.Add(new UserCategoryRelation()
                            {
                                Id = Guid.NewGuid(),
                                CategoryId = category.Id,
                                SelectDate = DateTime.Now,
                                UserId = userID
                            });
                    }
                }

                _dbContext.SaveChanges();
            }
            catch { }
        }

        public TransactionResult SaveUser(ProfileViewModel model, IFormFile headerImage, IFormFile avatarImage, IEnumerable<IFormFile> galleryImages)
        {
            try
            {
                var upload = new FormUpload(_environment.WebRootPath);

                var user = _dbContext.Users
                    .Include(i => i.District)
                    .Include(i => i.Province)
                    .Include(i => i.UserCategoryRelation)
                    .Include(i => i.UserExperienceImages)
                    .FirstOrDefault(i => i.SlugUrl == model.SlugUrl);

                if (user != null)
                {
                    //TODO Uncomment here
                    //if (headerImage.Length != 0)
                    //{
                    //    var imageResult = upload.SaveFile(headerImage, model.SlugUrl, "profileheader");

                    //    if (imageResult.Type == TransactionType.Success)
                    //        user.ProfileImageUrl = imageResult.Result as string;
                    //    else
                    //        return imageResult;
                    //}

                    //if (avatarImage.Length != 0)
                    //{
                    //    var imageResult = upload.SaveFile(avatarImage, model.SlugUrl, "profile");

                    //    if (imageResult.Type == TransactionType.Success)
                    //        user.AvatarUrl = imageResult.Result as string;
                    //    else
                    //        return imageResult;
                    //}

                    //foreach (var image in galleryImages)
                    //{
                    //    if (!(user.IsBoughtAnyToken.HasValue && !user.IsBoughtAnyToken.Value && user.UserExperienceImages.Count >= 5))
                    //        if (image.Length != 0)
                    //        {
                    //            var item = new UserExperienceImages();
                    //            var filename = string.Format("{0}-{1}", user.SlugUrl, DateTime.Now.ToString("yyMMddhhmmss_fff"));
                    //            var imageResult = upload.SaveFile(image, filename, "profilegallery");

                    //            if (imageResult.Type != TransactionType.Success)
                    //                return imageResult;

                    //            item.Url = imageResult.Result as string;
                    //            item.UserId = user.Id;
                    //            user.UserExperienceImages.Add(item);
                    //        }
                    //}

                    user.FullName = model.FullName;
                    user.Title = model.Title;
                    user.Email = model.Email;
                    user.PhoneNumber2 = model.Phone;
                    user.Description = model.Description;
                    user.Address = model.Address;
                    user.SmsNotAllowed = model.SMSNotAllowed;

                    if (model.ProvinceId != 0)
                    {
                        user.ProvinceId = model.ProvinceId;

                        if (model.DistrictId != 0)
                            user.DistrictId = model.DistrictId;
                        else
                            user.DistrictId = null;
                    }
                    else
                    {
                        user.ProvinceId = null;
                        user.DistrictId = null;
                    }

                    _dbContext.UserCategoryRelation.RemoveRange(user.UserCategoryRelation);

                    if (model.SelectedUserCategories != null)
                    {
                        var categories = _categoryServices.GetCategories();
                        foreach (var relation in model.SelectedUserCategories)
                        {
                            var category = categories.FirstOrDefault(i => i.SlugUrl == relation);

                            if (category != null)
                                user.UserCategoryRelation.Add(new UserCategoryRelation()
                                {
                                    Id = Guid.NewGuid(),
                                    CategoryId = category.Id,
                                    SelectDate = DateTime.Now,
                                    UserId = user.Id
                                });
                        }
                    }
                }

                _dbContext.SaveChanges();
                _notificationService.AddNotification(user.Id, NotificationTypes.ProfileSaved);

                ClearUserSession(user.Id);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public TransactionResult ChangeSmsSetting(string userID, bool smsNotAllowed)
        {
            try
            {
                var user = _dbContext.Users.FirstOrDefault(i => i.Id == userID);

                if (user != null)
                {
                    user.SmsNotAllowed = smsNotAllowed;
                    _dbContext.SaveChanges();

                    ClearUserSession(user.Id);

                    return new TransactionResult() { Type = TransactionType.Success };
                }

                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
            catch
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }
    }
}