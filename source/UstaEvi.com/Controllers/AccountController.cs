using Common.Entities;
using Common.Entities.Enums;
using Common.Utilities;
using Common.Utilities.Converters;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Account;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace UstaEvi.com.Controllers
{
    public class AccountController : BaseController
    {
        private ISession _session => _httpContextAccessor.HttpContext.Session;
        private readonly IHttpContextAccessor _httpContextAccessor;

        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IUserEngine _userEngine;
        private readonly IProjectEngine _projectEngine;

        public AccountController(
            IHttpContextAccessor httpContextAccessor,
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            INotificationService notificationService,
            ILogger logger,
            IContentEngine contentEngine,
            IProjectEngine projectEngine,
            IUserEngine userEngine
            ) : base(contentEngine)
        {
            _httpContextAccessor = httpContextAccessor;
            _userManager = userManager;
            _signInManager = signInManager;
            _notificationService = notificationService;
            _projectEngine = projectEngine;
            _userEngine = userEngine;
            _logger = logger;
        }

        [Route("giris/{model?}", Name = "login")]
        [HttpGet]
        public async Task<IActionResult> Login(string returnUrl = null)
        {
            SetPageContent("uyegiris");
            ViewData["ReturnUrl"] = returnUrl;

            if (_signInManager.IsSignedIn(User))
            {
                _session.Remove("LOGGED_USER" + _userManager.GetUserId(User));
                await _signInManager.SignOutAsync();
                return RedirectToRoute("login");
            }

            return View();
        }

        [Route("giris/{model?}", Name = "login")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            SetPageContent("uyegiris");
            ViewData["ReturnUrl"] = returnUrl;

            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, lockoutOnFailure: false);
                if (result.Succeeded)
                {
                    return RedirectToLocal(returnUrl);
                }
                else if (result.RequiresTwoFactor)
                {
                    return RedirectToRoute("verifycode", new { ReturnUrl = returnUrl });
                }
                else if (result.IsLockedOut)
                {
                    return Redirect("~/uyelikpasif");
                }

                ModelState.AddModelError(string.Empty, "E-mail adresiniz veya şifreniz hatalı.");
                return View(model);
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Route("cikis", Name = "logoff")]
        public async Task<IActionResult> LogOff()
        {
            _session.Remove("LOGGED_USER" + _userManager.GetUserId(User));
            await _signInManager.SignOutAsync();
            return RedirectToRoute("home");
        }

        [Route("kayit", Name = "registerOverview")]
        [HttpGet]
        public IActionResult RegisterOverview()
        {
            return View();
        }

        [Route("kayit/{type}/{model?}", Name = "register")]
        [HttpGet]
        public async Task<IActionResult> Register(string type)
        {
            var isCompany = type == "sirket";

            if (isCompany)
                SetPageContent("uyekayit-sirket");
            else
                SetPageContent("uyekayit");

            if (_signInManager.IsSignedIn(User))
            {
                _session.Remove("LOGGED_USER" + _userManager.GetUserId(User));
                await _signInManager.SignOutAsync();
                return RedirectToRoute("register");
            }

            return View(new RegisterViewModel()
            {
                IsCompany = isCompany,
                UserCategories = _projectEngine.GetUserCategories(),
            });
        }

        [Route("kayit/{type}/{model?}", Name = "register")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string type, RegisterViewModel model)
        {
            if (model.IsCompany)
                SetPageContent("uyekayit-sirket");
            else
                SetPageContent("uyekayit");

            if (ModelState.IsValid)
            {
                if (model.IsCompany && model.SelectedUserCategories == null)
                {
                    ModelState.AddModelError("", "En az bir adet hizmet vermek istediğiniz kategori seçmelisiniz.");
                }
                else
                {
                    var user = new User
                    {
                        FullName = model.FullName,
                        UserName = model.Email,
                        PhoneNumber = model.CellPhone,
                        Email = model.Email,
                        SlugUrl = GetUniqueSlugUrl(model.FullName),
                        TwoFactorEnabled = true,
                        PhoneNumberConfirmed = true,
                        LockoutEnabled = false,
                        CreatedDate = DateTime.Now
                    };

                    var existsPhoneNumber = _userManager.Users.Any(i => i.PhoneNumber == model.CellPhone);

                    if (!existsPhoneNumber)
                    {
                        var result = await _userManager.CreateAsync(user, model.Password);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            _userEngine.AfterRegistration(user.Id, SelectedUserCategories: model.SelectedUserCategories);

                            return RedirectToRoute("verifycode");
                        }
                        else if (result.Errors.Any(i => i.Code == "DuplicateUserName"))
                        {
                            var duplicateUserName = result.Errors.FirstOrDefault(i => i.Code == "DuplicateUserName");

                            if (duplicateUserName != null)
                                duplicateUserName.Description = string.Format("{0} adresli hesap bulunmaktadır!", model.Email);
                        }

                        AddErrors(result);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Kullandığınız cep telefonu numarası ile birden fazla kayıt yapamazsınız.");
                    }
                }
            }

            // If we got this far, something failed, redisplay form
            model.UserCategories = _projectEngine.GetUserCategories();
            return View(model);
        }

        [Route("sifreyenile/{model?}", Name = "resetpassword")]
        [HttpGet]
        public IActionResult ResetPassword(string userId, string code = null)
        {
            SetPageContent("resetpassword");

            ViewData["userId"] = userId;
            ViewData["code"] = code;

            return View();
        }

        [Route("sifreyenile/{model?}", Name = "resetpassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetPassword(ResetPasswordViewModel model)
        {
            SetPageContent("resetpassword");

            if (!ModelState.IsValid)
            {
                return View(model);
            }
            var user = await _userManager.FindByIdAsync(model.UserId);
            if (user == null)
            {
                // Don't reveal that the user does not exist
                ModelState.AddModelError("", "Kullanıcı kaydı bulunamadı. Lütfen tekrar deneyiniz.");
                return View(model);
            }
            var result = await _userManager.ResetPasswordAsync(user, model.Code, model.Password);
            if (result.Succeeded)
            {
                return RedirectToRoute("home");
            }

            AddErrors(result);
            return View();
        }

        [Route("sifremiunuttum/{model?}", Name = "forgetpassword")]
        [HttpGet]
        public IActionResult ForgetPassword()
        {
            SetPageContent("forgetpassword");

            return View();
        }

        [Route("sifremiunuttum/{model?}", Name = "forgetpassword")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgetPassword(ForgetPasswordViewModel model)
        {
            SetPageContent("forgetpassword");

            if (ModelState.IsValid)
            {
                var user = _userManager.Users.FirstOrDefault(i => i.PhoneNumber == model.CellPhone);
                if (user == null || !(await _userManager.IsPhoneNumberConfirmedAsync(user)))
                {
                    ModelState.AddModelError("", "Kullanıcı kaydı bulunamadı. Lütfen tekrar deneyiniz.");
                    return View(model);
                }

                var code = await _userManager.GeneratePasswordResetTokenAsync(user);

                var callbackUrl = Url.RouteUrl("resetpassword", new { userId = user.Id, code = code });

                _notificationService.AddNotification(user.Id, NotificationTypes.ForgetPassMail, param: callbackUrl);

                return SetTransactionResult(
                   string.Format("Şifre yenileme maili {0} adresine başarılı bir şekilde gönderilmiştir. Mailinize gelecek şifre yenileme linki ile şifrenizi değiştirebilirsiniz.", Utils.MaskEmail(user.Email)),
                    Url.RouteUrl("home"),
                    TransactionType.Success
                );
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [Route("koddogrulama/{model?}", Name = "verifycode")]
        [HttpGet]
        public async Task<IActionResult> VerifyCode(string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            var type = VerificationTypes.Registration;

            switch (type)
            {
                case VerificationTypes.Registration:
                    SetPageContent("verifycode");
                    break;

                case VerificationTypes.PasswordRenewal:
                    SetPageContent("verifycode-newpass");
                    break;
            }

            User user = null;
            var userId = _userManager.GetUserId(User);
            if (!string.IsNullOrWhiteSpace(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }
            else
            {
                user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user == null)
                {
                    var externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

                    if (externalLoginInfo != null)
                        user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
                }
            }

            if (user == null)
                return SetTransactionResult(
                    "Kullanıcı bulunamadı! Kayıt ekranına yönlendirileceksiniz.",
                    Url.RouteUrl("register"),
                    TransactionType.Error);

            switch (type)
            {
                case VerificationTypes.Registration:
                    if (user.PhoneNumberConfirmed && !user.TwoFactorEnabled)
                        return SetTransactionResult(
                             "Kullanıcı aktif! Üye giriş ekranına yönlendiriliyorsunuz.",
                             Url.RouteUrl("login"),
                             TransactionType.Error);

                    break;

                case VerificationTypes.PasswordRenewal:
                    break;
            }

            var reaminingSeconds = "180";
            if (user.VerifyCodeExpireDate.HasValue && user.VerifyCodeExpireDate.Value >= DateTime.Now)
            {
                reaminingSeconds = ((int)user.VerifyCodeExpireDate.Value.Subtract(DateTime.Now).TotalSeconds).ToString();
            }
            else
            {
                var code = Utils.GenereteSmsCode();
                user.VerifyCode = code;
                user.VerifyCodeExpireDate = DateTime.Now.AddMinutes(3);

                var updateResult = await _userManager.UpdateAsync(user);

                //ViewBag.Code = code; //TODO remove this

                var result = _notificationService.SendSMSVerificationNotification(type, user.PhoneNumber, code);

                if (!updateResult.Succeeded || result == null || !result.Contains("200"))
                {
                    throw new BaseException("Sms doğrulama sırasında beklenmedik bir hata oluştu! Lütfen tekrar deneyiniz.");
                }
            }

            return View(new VerifyCodeViewModel
            {
                ReaminingSeconds = reaminingSeconds,
                Type = type
            });
        }

        [Route("koddogrulama/{model?}", Name = "verifycode")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            switch (model.Type)
            {
                case VerificationTypes.Registration:
                    SetPageContent("verifycode");
                    break;

                case VerificationTypes.PasswordRenewal:
                    SetPageContent("verifycode-newpass");
                    break;
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            User user = null;
            ExternalLoginInfo externalLoginInfo = null;
            var userId = _userManager.GetUserId(User);
            var isExternal = false;
            var isTwoFactor = false;

            if (!string.IsNullOrWhiteSpace(userId))
            {
                user = await _userManager.FindByIdAsync(userId);
            }
            else
            {
                user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

                if (user != null)
                {
                    isTwoFactor = true;
                }
                else
                {
                    externalLoginInfo = await _signInManager.GetExternalLoginInfoAsync();

                    if (externalLoginInfo != null)
                    {
                        isExternal = true;
                        user = await _userManager.FindByLoginAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey);
                    }
                }
            }

            if (!(user.VerifyCodeExpireDate.HasValue && user.VerifyCodeExpireDate.Value >= DateTime.Now))
            {
                ModelState.AddModelError("", "Onay kodunu size ayrılan sürede girmediniz! Lütfen tekrar deneyiniz.");
                return View(model);
            }

            if (user.VerifyCode == model.Code)
            {
                if (!isExternal && !isTwoFactor)
                {
                    await _signInManager.SignInAsync(user, false);

                    if (_signInManager.IsSignedIn(User))
                    {
                        user.TwoFactorEnabled = false;
                        user.LockoutEnabled = false;
                        await _userManager.UpdateAsync(user);
                        _notificationService.AddNotification(user.Id, NotificationTypes.CellPhoneVerified);

                        return RedirectToLocal(returnUrl);
                    }

                    var isLockedOut = await _userManager.IsLockedOutAsync(user);
                    if (isLockedOut)
                    {
                        return Redirect("~/uyelikpasif");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Lütfen tekrar deneyiniz.");
                        return View(model);
                    }
                }

                if (isTwoFactor)
                {
                    user.TwoFactorEnabled = false;
                    user.LockoutEnabled = false;
                    await _userManager.UpdateAsync(user);

                    return SetTransactionResult(
                        "Üyeliğiniz aktif edilmiştir. Üye giriş ekranına yönlendirileceksiniz.",
                        Url.RouteUrl("login"),
                        TransactionType.Success
                        );
                }

                if (isExternal)
                {
                    user.TwoFactorEnabled = false;
                    user.LockoutEnabled = false;
                    await _userManager.UpdateAsync(user);

                    var result = await _signInManager.ExternalLoginSignInAsync(externalLoginInfo.LoginProvider, externalLoginInfo.ProviderKey, isPersistent: false);
                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                    if (result.IsLockedOut)
                    {
                        return Redirect("~/uyelikpasif");
                    }
                }
            }

            ModelState.AddModelError("", "Hatalı onay kodu girdiniz! Lütfen tekrar deneyiniz.");
            return View(model);
        }

        [Route("account/externallogin", Name = "externallogin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult ExternalLogin(string provider, string returnUrl = null)
        {
            // Request a redirect to the external login provider.
            var redirectUrl = Url.Action("ExternalLoginCallback", "Account", new { returnUrl = returnUrl });
            var properties = _signInManager.ConfigureExternalAuthenticationProperties(provider, redirectUrl);
            return new ChallengeResult(provider, properties);
        }

        //TODO change this after RTM version
        [Route("externallogincallback", Name = "externallogincallback")]
        [HttpGet]
        public async Task<IActionResult> ExternalLoginCallback(string returnUrl = null)
        {
            var info = await _signInManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return RedirectToAction(nameof(Login));
            }

            // Sign in the user with this external login provider if the user already has a login.
            var result = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider, info.ProviderKey, isPersistent: false);
            if (result.Succeeded)
            {
                return RedirectToLocal(returnUrl);
            }
            if (result.RequiresTwoFactor)
            {
                return RedirectToRoute("verifycode", new { returnUrl = returnUrl });
            }
            if (result.IsLockedOut)
            {
                return Redirect("~/uyelikpasif");
            }
            else
            {
                // If the user does not have an account, then ask the user to create an account.
                var loginProvider = info.LoginProvider;
                var email = info.Principal.FindFirstValue(ClaimTypes.Email);
                var name = info.Principal.FindFirstValue(ClaimTypes.Name);

                return RedirectToRoute("externalloginconfirm", new
                {
                    email = email,
                    name = name,
                    loginProvider = loginProvider,
                    returnUrl = returnUrl
                });
            }
        }

        [Route("baglanti/{model?}", Name = "externalloginconfirm")]
        public IActionResult ExternalLoginConfirmation(string email, string name, string loginProvider, string returnUrl)
        {
            var model = new ExternalLoginConfirmationViewModel
            {
                Email = email,
                FullName = name,
                Provider = loginProvider,
                ReturnUrl = returnUrl,
                UserCategories = _projectEngine.GetUserCategories()
            };

            SetPageContent(model.Provider + "-register");

            ViewData["Provider"] = model.Provider;

            return View(model);
        }

        [Route("baglanti/{model?}", Name = "externalloginconfirm")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model)
        {
            SetPageContent(model.Provider + "-register");

            ViewData["Provider"] = model.Provider;

            if (_signInManager.IsSignedIn(User))
            {
                return RedirectToLocal(model.ReturnUrl);
            }

            if (ModelState.IsValid)
            {
                var info = await _signInManager.GetExternalLoginInfoAsync();
                if (info == null)
                {
                    SetTransactionResult(
                        model.Provider + " ile bağlantı kurulamadı. Lütfen diğer giriş yöntemlerinden birini seçiniz.",
                        Url.RouteUrl("login"),
                        TransactionType.Error);
                }

                var user = new User
                {
                    UserName = model.Email,
                    Email = model.Email,
                    FullName = model.FullName,
                    PhoneNumber = model.CellPhone,
                    SlugUrl = GetUniqueSlugUrl(model.FullName),
                    TwoFactorEnabled = true,
                    PhoneNumberConfirmed = true,
                    LockoutEnabled = false,
                    CreatedDate = DateTime.Now
                };

                var existsPhoneNumber = _userManager.Users.Any(i => i.PhoneNumber == model.CellPhone);

                if (!existsPhoneNumber)
                {
                    var result = await _userManager.CreateAsync(user);
                    if (result.Succeeded)
                    {
                        result = await _userManager.AddLoginAsync(user, info);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);

                            _userEngine.AfterRegistration(user.Id, notificationDescription: model.Provider, SelectedUserCategories: model.SelectedUserCategories);

                            return RedirectToRoute("verifycode", new { returnUrl = model.ReturnUrl });
                        }
                    }
                    else if (result.Errors.Any(i => i.Code == "DuplicateUserName"))
                    {
                        var duplicateUserName = result.Errors.FirstOrDefault(i => i.Code == "DuplicateUserName");

                        if (duplicateUserName != null)
                            duplicateUserName.Description = string.Format("{0} adresli hesap bulunmaktadır!", model.Email);
                    }

                    AddErrors(result);
                }
                else
                {
                    ModelState.AddModelError("", "Kullandığınız cep telefonu numarası ile birden fazla kayıt yapamazsınız.");
                }

                model.UserCategories = _projectEngine.GetUserCategories();
                return View(model);
            }

            //PrivacyPolicy alanı bırakıldığı için birden fazlaysa sil.
            if (ModelState.ErrorCount > 1)
                ModelState.Clear();

            model.UserCategories = _projectEngine.GetUserCategories();
            return View(model);
        }

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

        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError(string.Empty, error.Description);
            }
        }

        private IActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToRoute("account");
            }
        }
    }
}