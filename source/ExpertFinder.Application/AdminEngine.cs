using Common.Entities;
using Common.Entities.Enums;
using Common.Utilities;
using Common.Utilities.Converters;
using Common.Web.IO;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.AdminViewModel;
using ExpertFinder.Common.Enums;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ExpertFinder.Application
{
    public class AdminEngine : IAdminEngine
    {
        #region Variables

        private static CultureInfo turkish = new CultureInfo("tr-TR");
        private const int PAGEITEMCOUNT = 10;
        private readonly teklifcepteDBContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        #endregion Variables

        #region Public Methods

        public AdminEngine(teklifcepteDBContext dbContext, UserManager<User> userManager, INotificationService notificationService, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _userManager = userManager;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        public TransactionResult ChangeContent(ContentsFormViewModel model)
        {
            try
            {
                var _content = new Content();
                _content.Id = model.Id;
                _content.Key = model.Key;
                _content.Title = model.Title;
                _content.Value = model.Value;

                _dbContext.Content.Attach(_content);
                _dbContext.Entry(_content).State = EntityState.Modified;
                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<User> GetAllEmploye()
        {
            var users = _userManager.GetUsersInRoleAsync("Admin").Result.OrderBy(i => i.FullName);
            return users;
        }

        async public Task<TransactionResult> AddEmploye(string userId, EmployeFormViewModel model)
        {
            try
            {
                var user = new User
                {
                    FullName = model.FullName,
                    UserName = model.Email,
                    PhoneNumber = model.CellPhone,
                    Email = model.Email,
                    EmailConfirmed = true,
                    SlugUrl = GetUniqueSlugUrl(model.FullName),
                    TwoFactorEnabled = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = bool.Parse(model.IsActive),
                    CreatedDate = DateTime.Now
                };

                var existsPhoneNumberOrEmail = _userManager.Users.Any(i => i.PhoneNumber == model.CellPhone || i.Email == model.Email);

                if (existsPhoneNumberOrEmail)
                {
                    return new TransactionResult() { Message = "Email veya Cep Telefonu başka bir kullanıcı tarafından kullanılmaktadır.", Type = TransactionType.Warning };
                }
                else
                {
                    string password = Utils.GeneratePassword(3, 3, 3);

                    var result = await _userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        user = await _userManager.FindByIdAsync(user.Id);

                        await _userManager.SetLockoutEnabledAsync(user, bool.Parse(model.IsActive));
                        await _userManager.AddToRoleAsync(user, "Admin");

                        _notificationService.AddNotification(user.Id, NotificationTypes.AdminInfoMail, param: password);

                        //_notificationService.SendAdminUserInfo(user.Id, user.Email, user.Email, password);
                    }
                }

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        async public Task<TransactionResult> UpdateEmploye(EmployeFormViewModel model)
        {
            try
            {
                User user = _userManager.FindByIdAsync(model.Id).Result;

                user.FullName = model.FullName;
                user.UserName = model.Email;
                user.PhoneNumber = model.CellPhone;
                user.Email = model.Email;
                var isActive = bool.Parse(model.IsActive);
                user.LockoutEnabled = isActive;
                if (isActive)
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                else
                    user.LockoutEnd = null;

                var existsPhoneNumberOrEmail = _userManager.Users.Any(i => (i.PhoneNumber == model.CellPhone || i.Email == model.Email) && i.Id != model.Id);

                if (existsPhoneNumberOrEmail)
                {
                    return new TransactionResult() { Message = "Email veya Cep Telefonu başka bir kullanıcı tarafından kullanılmaktadır.", Type = TransactionType.Warning };
                }
                else
                {
                    var result = await _userManager.UpdateAsync(user);

                    if (result.Succeeded)
                    {
                        _notificationService.AddNotification(user.Id, Common.Enums.NotificationTypes.UpdateAdmin);
                    }
                }

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<Campain> GetCampaign(bool? status)
        {
            if (status != null)
                return _dbContext.Campain.Where(i => i.IsActive == status);
            else
                return _dbContext.Campain.AsEnumerable();
        }

        public Campain GetCampaignDetail(int id)
        {
            return _dbContext.Campain.Where(i => i.Id == id).FirstOrDefault();
        }

        public TransactionResult AddCampaign(CampaignFormViewModel model, IFormFile headerImage)
        {
            try
            {
                string imgType = string.Empty;
                if (headerImage.Length > 0)
                {
                    imgType = headerImage.GetExtension();
                    model.Url = string.Format("{0}{1}", GetUniqueSlugUrlForCampaign(model.Title), imgType);
                }

                var campaign = new Campain
                {
                    Title = model.Title,
                    Description = model.Description,
                    TokenCount = model.TokenCount,
                    Price = model.Price,
                    Priority = model.Priority,
                    IsActive = bool.Parse(model.IsActive),
                    IsFree = bool.Parse(model.IsFree),
                    SlugUrl = GetUniqueSlugUrlForCampaign(model.Title),
                    Url = model.Url
                };

                _dbContext.Campain.Add(campaign);

                _dbContext.SaveChanges();

                if (headerImage.Length > 0)
                {
                    if (!headerImage.VerifyFileSize())
                    {
                        return new TransactionResult(code: 7001, message: "Dosya boyutu 1mb tan fazla olamaz. Lütfen boyutu daha 1mb altında bir dosya seçiniz.", type: TransactionType.Error);
                    }

                    UploadImage(headerImage, PageContentTypes.Campaign, campaign.Id.ToString(), campaign.Url);
                }

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public TransactionResult UpdateCampaign(CampaignFormViewModel model, IFormFile headerImage)
        {
            try
            {
                string imgType = string.Empty;
                if (headerImage.Length > 0)
                {
                    imgType = headerImage.GetExtension();
                    model.Url = string.Format("{0}{1}", GetUniqueSlugUrlForCampaign(model.Title), imgType);
                }

                var campaign = new Campain
                {
                    Id = model.Id,
                    Title = model.Title,
                    Description = model.Description,
                    TokenCount = model.TokenCount,
                    Price = model.Price,
                    Priority = model.Priority,
                    IsActive = bool.Parse(model.IsActive),
                    IsFree = bool.Parse(model.IsFree),
                    SlugUrl = GetUniqueSlugUrlForCampaign(model.Title),
                    Url = model.Url
                };

                _dbContext.Campain.Attach(campaign);
                _dbContext.Entry(campaign).State = EntityState.Modified;
                _dbContext.SaveChanges();

                if (headerImage.Length > 0)
                {
                    if (!headerImage.VerifyFileSize())
                    {
                        return new TransactionResult(code: 7001, message: "Dosya boyutu 1mb tan fazla olamaz. Lütfen boyutu daha 1mb altında bir dosya seçiniz.", type: TransactionType.Error);
                    }

                    UploadImage(headerImage, PageContentTypes.Campaign, campaign.Id.ToString(), campaign.Url);
                }

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public TransactionResult AddCategory(CategoryFormViewModel model, IFormFile headerImage)
        {
            try
            {
                string imgType = string.Empty;
                if (headerImage.Length > 0)
                {
                    imgType = headerImage.GetExtension();
                    model.ImageUrl = string.Format("{0}{1}", GetUniqueSlugUrlForCategory(model.Name), imgType);
                }

                var category = new Category
                {
                    Id = Guid.NewGuid(),
                    Name = model.Name,
                    ParentId = model.ParentId,
                    Priority = model.Priority,
                    Icon = model.Icon,
                    SlugUrl = GetUniqueSlugUrlForCategory(model.Name),
                    ImageUrl = model.ImageUrl,
                    IsUrun = model.IsUrun
                };

                _dbContext.Category.Add(category);

                _dbContext.SaveChanges();

                if (headerImage.Length > 0)
                {
                    if (!headerImage.VerifyFileSize())
                    {
                        return new TransactionResult(code: 7001, message: "Dosya boyutu 1mb tan fazla olamaz. Lütfen boyutu daha 1mb altında bir dosya seçiniz.", type: TransactionType.Error);
                    }

                    UploadImage(headerImage, PageContentTypes.Category, category.Id.ToString(), category.ImageUrl);
                }

                ClearCache("PopularCategories");
                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public TransactionResult UpdateCategory(CategoryFormViewModel model, IFormFile headerImage)
        {
            try
            {
                string imgType = string.Empty;
                if (headerImage.Length > 0)
                {
                    imgType = headerImage.GetExtension();
                    model.ImageUrl = string.Format("{0}{1}", GetUniqueSlugUrlForCategory(model.Name), imgType);
                }

                var category = new Category
                {
                    Id = model.Id,
                    Name = model.Name,
                    ParentId = model.ParentId,
                    Priority = model.Priority,
                    Icon = model.Icon != null ? model.Icon.Trim() : string.Empty,
                    SlugUrl = model.SlugUrl,
                    ImageUrl = model.ImageUrl,
                    IsUrun = model.IsUrun
                };

                _dbContext.Category.Attach(category);
                _dbContext.Entry(category).State = EntityState.Modified;
                _dbContext.SaveChanges();

                if (headerImage.Length > 0)
                {
                    if (!headerImage.VerifyFileSize())
                    {
                        return new TransactionResult(code: 7001, message: "Dosya boyutu 1mb tan fazla olamaz. Lütfen boyutu daha 1mb altında bir dosya seçiniz.", type: TransactionType.Error);
                    }

                    UploadImage(headerImage, PageContentTypes.Category, category.Id.ToString(), category.ImageUrl);
                }

                ClearCache("PopularCategories");

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<Category> GetCategories(bool? isRoot)
        {
            if (isRoot != null && isRoot == true)
                return _dbContext.Category.Where(i => i.ParentId == null).OrderBy(i => i.Name);
            else
                return _dbContext.Category.AsEnumerable().OrderBy(i => i.Name);
        }

        public Category GetCategory(Guid id)
        {
            return _dbContext.Category.Where(i => i.Id == id).FirstOrDefault();
        }

        public IEnumerable<Category> GetSubCategories(Guid parantId)
        {
            return _dbContext.Category.Where(i => i.ParentId == parantId).OrderBy(i => i.Name);
        }

        public IEnumerable<CategoryQuestions> GetCategoryQuestions(Guid categoryId)
        {
            return _dbContext.CategoryQuestions.Where(i => i.CategoryId == categoryId).OrderBy(i => i.Priority);
        }

        public TransactionResult AddCategoryQuestion(CategoryQuestions categoryQuestions)
        {
            try
            {
                _dbContext.CategoryQuestions.Add(categoryQuestions);

                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public TransactionResult RemoveAllCategoryQuestion(Guid categoryId)
        {
            try
            {
                IEnumerable<CategoryQuestions> categoryQuestions = _dbContext.CategoryQuestions.Where(i => i.CategoryId == categoryId);

                _dbContext.RemoveRange(categoryQuestions);

                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<Province> GetProvince()
        {
            return _dbContext.Province.AsEnumerable().OrderBy(i => i.Name);
        }

        public IEnumerable<District> GetDistrict(int province)
        {
            return _dbContext.District.Where(i => i.ProvinceId == province).OrderBy(i => i.Name);
        }

        public IEnumerable<User> SearchEmploye(
           string name,
           string email,
           int? provinceId = null,
           int? districtId = null,
           int page = 0)
        {
            var result = _dbContext.Users.OrderBy(i => i.FullName).AsEnumerable();

            if (!string.IsNullOrWhiteSpace(name))
                result = result.Where(i => i.FullName.ToLower(turkish).Contains(name.ToLower(turkish)));

            if (!string.IsNullOrWhiteSpace(email))
                result = result.Where(i => i.UserName == email);

            if (provinceId != null)
                result = result.Where(i => i.ProvinceId == provinceId);

            if (districtId != null)
                result = result.Where(i => i.DistrictId == districtId);

            result = result.Skip(page * PAGEITEMCOUNT).Take(PAGEITEMCOUNT);

            return result.ToList();
        }

        public User GetMemberDetail(string id)
        {
            return _dbContext.Users.Where(i => i.Id == id).FirstOrDefault();
        }

        public TransactionResult AddToken(Guid id, int tokenCount)
        {
            try
            {
                var user = _dbContext.Users.Where(i => i.Id == id.ToString()).FirstOrDefault();

                string campaignTitle = string.Format("{0} kullanıcısına {1} adet jeton kampanyası", user.FullName, tokenCount);

                var campaign = new Campain
                {
                    Title = campaignTitle,
                    TokenCount = tokenCount,
                    Price = 0,
                    Priority = 0,
                    IsActive = false,
                    IsFree = true,
                    SlugUrl = GetUniqueSlugUrlForCampaign(campaignTitle)
                };

                _dbContext.Campain.Add(campaign);

                _dbContext.SaveChanges();

                var bought = new BoughtHistory
                {
                    Id = Guid.NewGuid(),
                    CampainId = campaign.Id,
                    CreatedDate = DateTime.Now,
                    OrderAddress = user.Address,
                    UserId = user.Id.ToString(),
                    IsPaid = true
                };

                _dbContext.BoughtHistory.Add(bought);

                _dbContext.SaveChanges();

                _notificationService.AddNotification(user.Id, NotificationTypes.AdminToken);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public TransactionResult UpdateEmployeStatus(Guid id, string LockoutEnabled)
        {
            try
            {
                var user = _dbContext.Users.Where(i => i.Id == id.ToString()).FirstOrDefault();

                if (LockoutEnabled == "True")
                {
                    user.LockoutEnabled = false;
                    user.LockoutEnd = null;
                }
                else
                {
                    user.LockoutEnd = DateTimeOffset.MaxValue;
                    user.LockoutEnabled = true;
                }

                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<Offer> GetMemberOffers(string id)
        {
            return _dbContext.Offer
                .Include(i => i.Item.Category.Parent)
                .Where(i => i.UserId == id);
        }

        public List<Category> GetMemberCategoryRelationList(string id)
        {
            return _dbContext.UserCategoryRelation
                  .Include(i => i.Category)
                  .Include(i => i.Category.Parent)
                  .Where(i => i.UserId == id)
                  .Select(i => i.Category)
                  .OrderBy(i => i.Name)
                  .ToList();
        }

        public IEnumerable<Offer> GetItemOffers(string offerId)
        {
            if (!string.IsNullOrWhiteSpace(offerId))
            {
                var offer = _dbContext.Offer.Where(i => i.Id == new Guid(offerId)).FirstOrDefault();

                if (offer != null)
                {
                    var itemOffers = _dbContext.Offer
                                     .Include(i => i.User).
                                     Where(i => i.ItemId == offer.ItemId)
                                     .OrderByDescending(i => i.IsWinner)
                                     .AsEnumerable();

                    return itemOffers;
                }
                else
                    return null;
            }
            else
                return null;
        }

        public Item GetItemByOfferId(string offerId)
        {
            if (!string.IsNullOrWhiteSpace(offerId))
            {
                var itemId = _dbContext.Offer
                            .Where(i => i.Id == new Guid(offerId))
                            .Select(i => i.ItemId)
                            .FirstOrDefault();

                if (itemId != null)
                {
                    var item = _dbContext.Item.Where(i => i.Id == itemId)
                                .Include(i => i.Category)
                                .Include(i => i.Category.Parent)
                                .Include(i => i.Province)
                                .Include(i => i.District)
                                .Include(i => i.User)
                                .FirstOrDefault();

                    return item;
                }
            }

            return null;
        }

        public IEnumerable<Offer> GetItemOffersByItemId(string itemId)
        {
            if (!string.IsNullOrWhiteSpace(itemId))
            {
                var itemOffers = _dbContext.Offer
                                 .Include(i => i.User)
                                 .Where(i => i.ItemId == new Guid(itemId))
                                 .OrderByDescending(i => i.IsWinner)
                                 .AsEnumerable();

                return itemOffers;
            }
            else
                return null;
        }

        public Item GetItemById(string itemId)
        {
            if (!string.IsNullOrWhiteSpace(itemId))
            {
                var item = _dbContext.Item.Where(i => i.Id == new Guid(itemId))
                            .Include(i => i.Category)
                            .Include(i => i.Category.Parent)
                            .Include(i => i.Province)
                            .Include(i => i.District)
                            .Include(i => i.User)
                            .FirstOrDefault();

                return item;
            }
            else
                return null;
        }

        public IEnumerable<User> GetUserByName(string username)
        {
            var userList = _dbContext.Users.Where(i => i.FullName.ToLower(turkish).Contains(username.ToLower(turkish))).AsEnumerable();

            return userList;
        }

        public IEnumerable<User> GetUserByEmail(string useremail)
        {
            var userList = _dbContext.Users.Where(i => i.UserName.Contains(useremail)).AsEnumerable();

            return userList;
        }

        public IEnumerable<Category> GetRootCategoriesByName(string categoryName)
        {
            var categoryList = _dbContext.Category.Where(i => i.ParentId == null && i.Name.ToLower(turkish).Contains(categoryName.ToLower(turkish))).AsEnumerable();

            return categoryList;
        }

        public IEnumerable<Category> GetSubCategoriesByName(Guid parantId, string subCategoryName)
        {
            return _dbContext.Category.Where(i => i.ParentId == parantId && i.Name.ToLower(turkish).Contains(subCategoryName.ToLower(turkish))).OrderBy(i => i.Name);
        }

        public IEnumerable<Offer> SearchOffer(
            string user,
            string userEmail,
            string category,
            string subCategory,
            int page = 0)
        {
            var result = _dbContext.Offer
                            .Include(i => i.Item)
                            .Include(i => i.User)
                            .Include(i => i.Item.Category)
                            .Include(i => i.Item.Category.Parent)
                            .Include(i => i.Item.User)
                            .OrderBy(i => i.OfferDate)
                            .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(user))
                result = result.Where(i => i.User.FullName.ToLower(turkish).Contains(user.ToLower(turkish)));

            if (!string.IsNullOrWhiteSpace(userEmail))
                result = result.Where(i => i.User.UserName == userEmail);

            if (!string.IsNullOrWhiteSpace(category))
                result = result.Where(i => i.Item.Category.ParentId == new Guid(category));

            if (!string.IsNullOrWhiteSpace(subCategory))
                result = result.Where(i => i.Item.CategoryId == new Guid(subCategory));

            result = result.Skip(page * PAGEITEMCOUNT).Take(PAGEITEMCOUNT);

            return result.ToList();
        }

        public IEnumerable<Item> SearchItem(
            string user,
            string userEmail,
            string category,
            string subCategory,
            string status,
            int page = 0)
        {
            var result = _dbContext.Item
                            .Include(i => i.Category)
                            .Include(i => i.Category.Parent)
                            .Include(i => i.User)
                            .Include(i => i.Offer)
                            .OrderBy(i => i.AddedDate)
                            .AsEnumerable();

            if (!string.IsNullOrWhiteSpace(user))
                result = result.Where(i => i.User.FullName.ToLower(turkish).Contains(user.ToLower(turkish)));

            if (!string.IsNullOrWhiteSpace(userEmail))
                result = result.Where(i => i.User.UserName == userEmail);

            if (!string.IsNullOrWhiteSpace(category))
                result = result.Where(i => i.Category.ParentId == new Guid(category));

            if (!string.IsNullOrWhiteSpace(subCategory))
                result = result.Where(i => i.CategoryId == new Guid(subCategory));

            if (!string.IsNullOrWhiteSpace(status))
                result = result.Where(i => i.StatusID == int.Parse(status));

            result = result.Skip(page * PAGEITEMCOUNT).Take(PAGEITEMCOUNT);

            return result.ToList();
        }

        public int GetMemberTokenCount(string userId)
        {
            int? result = _dbContext.BoughtHistory.Where(i => i.UserId == userId && i.IsPaid).Select(i => i.Campain.TokenCount).Where(i => i.HasValue).AsEnumerable().Sum(i => i.Value)
                        - _dbContext.SpentHistory.Where(i => i.UserId == userId && i.TokenSpent.HasValue).AsEnumerable().Sum(i => i.TokenSpent.Value);

            return result.HasValue ? result.Value : 0;
        }

        public IEnumerable<Item> GetMemberItems(string userId)
        {
            if (!string.IsNullOrWhiteSpace(userId))
            {
                var itemOffers = _dbContext.Item
                                 .Include(i => i.Category)
                                 .Include(i => i.Category.Parent)
                                 .Where(i => i.UserId == userId)
                                 .OrderByDescending(i => i.AddedDate)
                                 .AsEnumerable();

                return itemOffers;
            }
            else
                return null;
        }

        public TransactionResult UpdateItemStatus(Guid id)
        {
            try
            {
                var item = _dbContext.Item.Where(i => i.Id == id).FirstOrDefault();

                item.StatusID = (int)StatusTypes.Declined;

                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }

        public IEnumerable<User> GetUserForSMS(int? province, int? district, string category, string subCategory)
        {
            var userList = _dbContext.Users
                           .Where(i => i.SmsNotAllowed == false || i.SmsNotAllowed == null)
                           .Include(i => i.UserCategoryRelation)
                           .AsEnumerable();

            if (district != null)
            {
                userList = userList.Where(i => i.DistrictId == district).AsEnumerable();
            }
            else if (province != null)
            {
                userList = userList.Where(i => i.ProvinceId == province).AsEnumerable();
            }

            if (!string.IsNullOrEmpty(subCategory))
            {
                userList = userList.Where(i => i.UserCategoryRelation.Any(x => x.CategoryId == new Guid(subCategory))).AsEnumerable();
            }
            else if (!string.IsNullOrEmpty(category))
            {
                userList = userList.Where(i => i.UserCategoryRelation.Any(x => x.CategoryId == new Guid(category))).AsEnumerable();
            }

            return userList;
        }

        #endregion Public Methods

        #region Private Methods

        private string GetUniqueSlugUrl(string FullName, string slugUrl = null, int index = 1)
        {
            if (string.IsNullOrWhiteSpace(slugUrl))
                slugUrl = UrlConverter.ToUrlSlug(FullName);

            var hasSlug = _userManager.Users.Any(i => i.SlugUrl == slugUrl);

            if (hasSlug)
                return GetUniqueSlugUrl(FullName, UrlConverter.ToUrlSlug(string.Format("{0}-{1}", FullName, index++)), index++);
            else
                return UrlConverter.ToUrlSlug(slugUrl);
        }

        private string GetUniqueSlugUrlForCampaign(string CampaignName, string slugUrl = null, int index = 1)
        {
            if (string.IsNullOrWhiteSpace(slugUrl))
                slugUrl = UrlConverter.ToUrlSlug(CampaignName);

            var hasSlug = _dbContext.Campain.Any(i => i.SlugUrl == slugUrl);

            if (hasSlug)
                return GetUniqueSlugUrl(CampaignName, UrlConverter.ToUrlSlug(string.Format("{0}-{1}", CampaignName, index++)), index++);
            else
                return UrlConverter.ToUrlSlug(slugUrl);
        }

        private string GetUniqueSlugUrlForCategory(string CategoryName, string slugUrl = null, int index = 1)
        {
            if (string.IsNullOrWhiteSpace(slugUrl))
                slugUrl = UrlConverter.ToUrlSlug(CategoryName);

            var hasSlug = _dbContext.Category.Any(i => i.SlugUrl == slugUrl);

            if (hasSlug)
                return GetUniqueSlugUrl(CategoryName, UrlConverter.ToUrlSlug(string.Format("{0}-{1}", CategoryName, index++)), index++);
            else
                return UrlConverter.ToUrlSlug(slugUrl);
        }

        async private Task UploadImage(IFormFile headerImage, PageContentTypes pageContentType, string id, string url)
        {
            string uiUrl = UIUrl;

            using (var httpClient = new HttpClient())
            {
                try
                {
                    var content = new MultipartFormDataContent();

                    content.Add(headerImage.ToStreamContent(), "file", url);

                    var result = await httpClient.PostAsync(
                        new Uri(string.Format("{0}/api/JSON/UploadImage/{1}/{2}/{3}",
                        uiUrl,
                        (int)pageContentType,
                        id,
                        url)),
                        content);
                }
                catch (Exception ex)
                {
                }
            }
        }

        private string UIUrl
        {
            get { return _configuration["Frontend:Domain"]; }
        }

        async private void ClearCache(string parameters)
        {
            using (var httpClient = new HttpClient())
            {
                try
                {
                    await httpClient.GetStringAsync(new Uri(string.Format("{0}/api/JSON/ClearCache/{1}", UIUrl, parameters)));
                }
                catch (Exception ex)
                {
                }
            }
        }

        #endregion Private Methods
    }
}