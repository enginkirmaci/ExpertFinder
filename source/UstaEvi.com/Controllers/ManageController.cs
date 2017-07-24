using Common.Entities.Enums;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Manage;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace UstaEvi.com.Controllers
{
    [Authorize]
    public class ManageController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly IUserEngine _userEngine;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IProjectEngine _projectEngine;

        public ManageController(
           UserManager<User> userManager,
           SignInManager<User> signInManager,
           IUserEngine userEngine,
           IProjectEngine projectEngine,
           INotificationService notificationService,
           ILogger logger,
           IContentEngine contentEngine
           ) : base(contentEngine)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _userEngine = userEngine;
            _projectEngine = projectEngine;
            _notificationService = notificationService;
            _logger = logger;
        }

        [Route("hesabim/{model?}", Name = "account")]
        [HttpGet]
        async public Task<IActionResult> Index()
        {
            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            return View(new ManageIndexViewModel()
            {
                UserCategories = user.UserCategoryRelation,
                Notifications = _notificationService.GetNotifications(_userManager.GetUserId(User))
            });
        }

        [Route("bildirimlerim/{id}", Name = "redirectNotification")]
        public IActionResult RedirectNotification(Guid id)
        {
            var notification = _notificationService.GetNotification(id);

            if (notification != null)
            {
                switch (notification.NotificationTypeId)
                {
                    case (int)NotificationTypes.CreateOffer:
                    case (int)NotificationTypes.ReceivedOffer:
                    case (int)NotificationTypes.AcceptOffer:
                    case (int)NotificationTypes.ReceivedAcceptOffer:
                    case (int)NotificationTypes.CreateItem:
                    case (int)NotificationTypes.EditItem:
                        if (notification.ItemID.HasValue)
                        {
                            var item = _projectEngine.GetItem(notification.ItemID.Value);

                            return RedirectToRoute("viewItem", new { category = item.Category.SlugUrl, slugurl = item.SlugUrl });
                        }
                        break;
                }
            }

            return RedirectToRoute("account");
        }

        [Route("profilim/{slugUrl}/{model?}", Name = "profile")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Profile(string slugUrl)
        {
            return View(new ProfileViewModel()
            {
                SlugUrl = slugUrl
            });
        }

        [Route("profilim/{slugUrl}/duzenle/{model?}", Name = "profileEdit")]
        [HttpGet]
        async public Task<IActionResult> ProfileEdit(string slugUrl)
        {
            SetPageContent("profileEdit");

            return View(await getProfileViewModel(slugUrl));
        }

        [Route("profilim/{slugUrl}/duzenle/{model?}", Name = "profileEdit")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ProfileEdit(string slugUrl, ProfileViewModel model)
        {
            SetPageContent("profileEdit");

            IFormFile headerImage = Request.Form.Files.GetFile("headerImage");
            IFormFile avatarImage = Request.Form.Files.GetFile("avatarImage");

            var galleryImages = Request.Form.Files.GetFiles("galleryImage");

            if (ModelState.IsValid)
            {
                var result = _userEngine.SaveUser(model, headerImage, avatarImage, galleryImages);

                switch (result.Type)
                {
                    case TransactionType.Error:
                        ModelState.AddModelError("", result.Message);
                        return View(getProfileViewModel(model));

                    case TransactionType.Success:
                        return RedirectToRoute("profile", new { slugUrl = slugUrl });
                }
            }

            SetValidationErrorMessage();

            // If we got this far, something failed, redisplay form
            return View(getProfileViewModel(model));
        }

        private void SetValidationErrorMessage()
        {
            ModelStateEntry entry = null;

            if (ModelState.TryGetValue("DistrictId", out entry) && entry.Errors.Count > 0)
            {
                entry.Errors.Clear();
                entry.Errors.Add("İlçe seçmelisiniz.");
            }

            if (ModelState.TryGetValue("ProvinceId", out entry) && entry.Errors.Count > 0)
            {
                entry.Errors.Clear();
                entry.Errors.Add("İl seçmelisiniz.");
            }
        }

        private async Task<ProfileViewModel> getProfileViewModel(string slugUrl)
        {
            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            return new ProfileViewModel()
            {
                ProfileImageUrl = user.ProfileImageUrl,
                AvatarUrl = user.AvatarUrl,
                FullName = user.FullName,
                Title = user.Title,
                Email = user.Email,
                CellPhone = user.PhoneNumber,
                Phone = user.PhoneNumber2,
                SlugUrl = slugUrl,
                Description = user.Description,
                Provinces = _projectEngine.GetProvincesSelectList(),
                ProvinceId = user.ProvinceId.HasValue ? user.ProvinceId.Value : 0,
                Districts = _projectEngine.GetDistrictsSelectList(user.ProvinceId),
                DistrictId = user.DistrictId.HasValue ? user.DistrictId.Value : 0,
                Address = user.Address,
                UserCategories = _projectEngine.GetUserCategories(),
                SelectedUserCategories = user.UserCategoryRelation.Select(i => i.Category.SlugUrl),
                UserExperienceImages = _userEngine.GetGallery(_userManager.GetUserId(User)),
                SMSNotAllowed = user.SmsNotAllowed
            };
        }

        private ProfileViewModel getProfileViewModel(ProfileViewModel model)
        {
            return new ProfileViewModel()
            {
                ProfileImageUrl = model.ProfileImageUrl,
                AvatarUrl = model.AvatarUrl,
                FullName = model.FullName,
                Title = model.Title,
                Email = model.Email,
                CellPhone = model.CellPhone,
                Phone = model.Phone,
                SlugUrl = model.SlugUrl,
                Description = model.Description,
                Provinces = _projectEngine.GetProvincesSelectList(),
                ProvinceId = model.ProvinceId,
                Districts = _projectEngine.GetDistrictsSelectList(model.ProvinceId),
                DistrictId = model.DistrictId,
                Address = model.Address,
                UserCategories = _projectEngine.GetUserCategories(),
                SelectedUserCategories = model.SelectedUserCategories,
                UserExperienceImages = _userEngine.GetGallery(_userManager.GetUserId(User))
            };
        }

        [Route("tekliflerim/{model?}", Name = "offers")]
        async public Task<IActionResult> Offers(int? FilterTypeId)
        {
            SetPageContent("offers");

            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            var filterType = FilterTypeId != null ? (OfferFilterTypes)FilterTypeId : OfferFilterTypes.All;
            var model = new OffersViewModel()
            {
                Offers = _projectEngine.GetUserOffers(user.Id, filterType),
                FilterTypeId = (int)filterType,
                FilterTypes = _projectEngine.GetOfferFilterTypes()
            };

            return View(model);
        }

        //offerList
        [Route("tekliflerim/offerList/{model?}", Name = "offerList")]
        [HttpPost]
        async public Task<IActionResult> Offers(OffersViewModel model, string acceptButton, string rateButton, string rateValue, string rateComment)
        {
            if (!string.IsNullOrWhiteSpace(rateButton))
            {
                var result = _projectEngine.CommentItem(rateButton, rateValue, rateComment);

                if (result.Type == TransactionType.Success)
                {
                    if (model != null)
                        return RedirectToRoute("offers", new { FilterTypeId = model.FilterTypeId });
                    else
                        return RedirectToRoute("offers");
                }
                else
                {
                    SetPageContent("offers");

                    var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

                    var filterType = model != null ? (OfferFilterTypes)model.FilterTypeId : OfferFilterTypes.All;
                    model.Offers = _projectEngine.GetUserOffers(user.Id, filterType);
                    model.FilterTypes = _projectEngine.GetOfferFilterTypes();

                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }

            if (model != null)
                return RedirectToRoute("offers", new { FilterTypeId = model.FilterTypeId });
            else
                return RedirectToRoute("offers");
        }

        [Route("hizmet-isteklerim/", Name = "projects")]
        async public Task<IActionResult> Projects(int? FilterTypeId)
        {
            SetPageContent("projects");

            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            var filterType = FilterTypeId != null ? (ProjectFilterTypes)FilterTypeId : ProjectFilterTypes.Open;
            var model = new ProjectsViewModel()
            {
                Items = _projectEngine.GetUserItems(user.Id, filterType),
                FilterTypeId = (int)filterType,
                FilterTypes = _projectEngine.GetProjectFilterTypes()
            };

            return View(model);
        }

        [Route("jetonlarim/", Name = "tokens")]
        async public Task<IActionResult> Tokens()
        {
            SetPageContent("tokens");

            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            var model = new TokensViewModel()
            {
                Items = _projectEngine.GetUserTokens(user.Id),
                TokenCount = _userEngine.CurrentUserToken(_userManager.GetUserId(User))
            };

            return View(model);
        }

        [Route("ayarlarim/{model?}", Name = "settings")]
        [HttpGet]
        async public Task<IActionResult> Settings(string message, bool isOk)
        {
            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            var userLogins = await _userManager.GetLoginsAsync(user);
            var otherLogins = _signInManager.GetExternalAuthenticationSchemes().Where(auth => userLogins.All(ul => auth.AuthenticationScheme != ul.LoginProvider)).ToList();
            ViewData["ShowRemoveButton"] = user.PasswordHash != null || userLogins.Count > 1;

            return View(new SettingsViewModel
            {
                User = user,
                HasPassword = await _userManager.HasPasswordAsync(user),
                CurrentLogins = userLogins,
                OtherLogins = otherLogins,
                SmsNotAllowed = user.SmsNotAllowed.HasValue && user.SmsNotAllowed.Value
            });
        }

        [Route("manage/linklogin", Name = "linkLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult LinkLogin(string provider)
        {
            // Request a redirect to the external login provider to link a login for the current user
            var redirectUrl = Url.Action("LinkLoginCallback", "Manage");
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl, _userManager.GetUserId(User));
            return new ChallengeResult(provider, properties);
        }

        [Route("manage/linklogincallback", Name = "linkLoginCallback")]
        [HttpGet]
        public async Task<ActionResult> LinkLoginCallback()
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            var info = await _signInManager.GetExternalLoginInfoAsync(_userManager.GetUserId(User));
            if (info == null)
            {
                TempData["message"] = "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";
                TempData["messagesuccess"] = false;
                return RedirectToRoute("settings");
            }

            var result = await _userManager.AddLoginAsync(user, info);

            var message = string.Empty;

            if (!result.Succeeded)
            {
                message = string.Join(";", result.Errors.Select(i => i.Code));
                TempData["message"] = message
                    .Replace("LoginAlreadyAssociated", string.Format("{0} hesabınız başka bir kullanıcıya bağlı olduğu için doğrulama işlemi yapılamadı.", info.LoginProvider));
                TempData["messagesuccess"] = false;
            }
            else
                _notificationService.AddNotification(user.Id, NotificationTypes.LinkExternalProvider, param: info.LoginProvider);

            return RedirectToRoute("settings");
        }

        [Route("manage/removelogin", Name = "removeLogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveLogin(IEnumerable<RemoveLoginViewModel> account, string RemoveButton)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var loginProvider = RemoveButton.Split(':')[0];
            var providerKey = RemoveButton.Split(':')[1];

            var result = await _userManager.RemoveLoginAsync(user, loginProvider, providerKey);
            var message = string.Empty;

            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(user, isPersistent: false);
                _userEngine.ClearUserSession(_userManager.GetUserId(User));
                _notificationService.AddNotification(user.Id, NotificationTypes.UnLinkExternalProvider, param: loginProvider);

                return RedirectToRoute("settings");
            }
            else
            {
                TempData["message"] = string.Join(";", result.Errors.Select(i => i.Description)); ;
                TempData["messagesuccess"] = false;
            }

            TempData["message"] = "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";
            TempData["messagesuccess"] = false;
            return RedirectToRoute("settings");
        }

        [Route("manage/changepassword", Name = "changePassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(";", ModelState.Values.Select(i => i.Errors).Select(i => i.FirstOrDefault()).Where(i => i != null).Select(i => i.ErrorMessage));
                TempData["message"] = message;
                TempData["passwordmessagesuccess"] = false;
                return RedirectToRoute("settings");
            }

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user != null)
            {
                var result = await _userManager.ChangePasswordAsync(user, model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _userEngine.ClearUserSession(_userManager.GetUserId(User));
                    _notificationService.AddNotification(user.Id, NotificationTypes.ChangedPassword);

                    TempData["message"] = "Şifreniz değiştirilmiştir.";
                    TempData["passwordmessagesuccess"] = true;
                    return RedirectToRoute("settings");
                }

                var message = string.Join(";", result.Errors.Select(i => i.Description));
                TempData["message"] = message;
                TempData["passwordmessagesuccess"] = false;
                return RedirectToRoute("settings");
            }

            TempData["message"] = "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";
            TempData["passwordmessagesuccess"] = false;
            return RedirectToRoute("settings");
        }

        [Route("manage/setpassword", Name = "setPassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetPassword(SettingsViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var message = string.Join(";", ModelState.Values.Select(i => i.Errors).Select(i => i.FirstOrDefault()).Where(i => i != null).Select(i => i.ErrorMessage));
                TempData["message"] = message;
                TempData["passwordmessagesuccess"] = false;
                return RedirectToRoute("settings");
            }

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));
            if (user != null)
            {
                var result = await _userManager.AddPasswordAsync(user, model.NewPassword);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    _userEngine.ClearUserSession(_userManager.GetUserId(User));
                    _notificationService.AddNotification(user.Id, NotificationTypes.SetPassword);
                    TempData["message"] = "Şifreniz oluşturulmuştur.";
                    TempData["passwordmessagesuccess"] = true;
                    return RedirectToRoute("settings");
                }

                var message = string.Join(";", result.Errors.Select(i => i.Description));
                TempData["message"] = message;
                TempData["passwordmessagesuccess"] = false;
                return RedirectToRoute("settings");
            }

            TempData["passwordmessagesuccess"] = false;
            return RedirectToRoute("settings");
        }

        [Route("manage/verifyemail", Name = "verifyEmail")]
        [HttpGet]
        public async Task<ActionResult> VerifyEmail()
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);

            var url = Url.Action("VerifyEmailConfirm", "Manage", new { code = code });

            _notificationService.AddNotification(_userManager.GetUserId(User), NotificationTypes.SendMailVerify, param: url);

            //var result = _notificationService.SendMailVerify(user.Email, url);
            //if (result.Type == TransactionType.Success)
            //{
            TempData["message"] = "Onay mailiniz yollanmıştır. Lütfen mailde gelen link üzerinden onaylama işlemini tamamlayınız.";
            TempData["messagesuccess"] = true;
            return RedirectToRoute("settings");

            //}
            //else
            //{
            //    TempData["message"] = result.Message;
            //    TempData["messagesuccess"] = false;
            //    return RedirectToRoute("settings");
            //}
        }

        [Route("manage/verifyemailconfirm", Name = "verifyEmailConfirm")]
        [HttpGet]
        public async Task<ActionResult> VerifyEmailConfirm(string code)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            if (user != null)
            {
                var result = await _userManager.ConfirmEmailAsync(user, code);
                if (result.Succeeded)
                {
                    _userEngine.ClearUserSession(_userManager.GetUserId(User));
                    _notificationService.AddNotification(user.Id, NotificationTypes.EmailVerified);

                    TempData["message"] = "Email adresiniz onaylanmıştır.";
                    TempData["messagesuccess"] = true;
                    return RedirectToRoute("settings");
                }
            }

            TempData["message"] = "Email onaylama kodunuz geçersiz. Lütfen tekrar deneyiniz.";
            TempData["messagesuccess"] = false;
            return RedirectToRoute("settings");
        }

        [Route("manage/smssetting", Name = "smssetting")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SMSSetting(SettingsViewModel model)
        {
            var result = _userEngine.ChangeSmsSetting(_userManager.GetUserId(User), model.SmsNotAllowed);
            if (result.Type == TransactionType.Success)
            {
                TempData["message"] = "Sms ayarlarınız değiştirilmiştir.";
                TempData["smssettingmessagesuccess"] = true;
                return RedirectToRoute("settings");
            }

            TempData["message"] = "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.";
            TempData["smssettingmessagesuccess"] = false;
            return RedirectToRoute("settings");
        }
    }
}