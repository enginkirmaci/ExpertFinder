using Common.Entities.Enums;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.ViewModels.Campaigns;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UstaEvi.com.Controllers
{
    [Authorize]
    public class CampaignsController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserEngine _userEngine;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IProjectEngine _projectEngine;
        private readonly ICampaignEngine _campaignEngine;

        public CampaignsController(
            UserManager<User> userManager,
           IUserEngine userEngine,
           IProjectEngine projectEngine,
           INotificationService notificationService,
           ILogger logger,
           IContentEngine contentEngine,
           ICampaignEngine campaignEngine
           ) : base(contentEngine)
        {
            _userManager = userManager;
            _userEngine = userEngine;
            _projectEngine = projectEngine;
            _campaignEngine = campaignEngine;
            _notificationService = notificationService;
            _logger = logger;
        }

        [Route("kampanyalar", Name = "campaign")]
        [AllowAnonymous]
        public IActionResult Campaign()
        {
            SetPageContent("campaign");

            return View(new CampaignViewModel()
            {
                Items = _campaignEngine.GetCampaigns()
            });
        }

        [Route("odeme/{slugurl}", Name = "payment")]
        [HttpGet]
        public IActionResult Payment(string slugurl)
        {
            SetPageContent("payment");

            return View(new PaymentViewModel()
            {
                Campaign = _campaignEngine.GetCampaign(slugurl),
                Months = GetMonths(),
                Years = GetYears()
            });
        }

        [Route("odeme/{slugurl}", Name = "payment")]
        [HttpPost]
        async public Task<IActionResult> Payment(PaymentViewModel model, string slugurl)
        {
            SetPageContent("payment");

            model.Campaign = _campaignEngine.GetCampaign(slugurl);
            model.Months = GetMonths();
            model.Years = GetYears();

            if (ModelState.IsValid)
            {
                var userIP = Request.Headers["X-Forwarded-For"].ToString();

                //TODO Uncomment here
                //var result = await _campaignEngine.PayCampain(_userManager.GetUserId(User), model, userIP.Split(':').FirstOrDefault());

                //switch (result.Type)
                //{
                //    case TransactionType.Error:
                //        ModelState.AddModelError("", result.Message);
                //        return View(model);

                //    case TransactionType.Success:
                //        return Content(result.Message, "text/html");
                //}
            }

            ModelState.Clear();
            if (Request.HasFormContentType)
            {
                if (Request.Form.ContainsKey("ResponseMessage"))
                    ModelState.AddModelError("", Request.Form["ResponseMessage"]);
            }
            else
                ModelState.AddModelError("", "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.");

            return View(model);
        }

        [Route("odemetamamlandi/", Name = "paymentsuccess")]
        [AllowAnonymous]
        public IActionResult PaymentSuccess()
        {
            SetPageContent("paymentsuccess");

            if (Request.HasFormContentType)
            {
                var str = string.Empty;

                foreach (var item in Request.Form.Keys)
                    str += item + ":" + Request.Form[item] + ";" + Environment.NewLine;

                var merchantOrderId = Request.Form["MerchantOrderId"];

                var result = _campaignEngine.PaymentComplete(str, merchantOrderId);

                switch (result.Type)
                {
                    case TransactionType.Error:
                        ModelState.AddModelError("", result.Message);
                        return View();

                    case TransactionType.Success:
                        return View(new PaymentSuccessViewModel()
                        {
                            Campaign = _campaignEngine.GetCampaignByOrderId(merchantOrderId)
                        });
                }
            }

            return RedirectToRoute("home");
        }

        private IEnumerable<SelectListItem> GetMonths()
        {
            var result = new List<SelectListItem>();

            result.Add(new SelectListItem() { Value = "", Text = "Ay" });
            result.Add(new SelectListItem() { Value = "01", Text = "01 (Ocak)" });
            result.Add(new SelectListItem() { Value = "02", Text = "02 (Şubat)" });
            result.Add(new SelectListItem() { Value = "03", Text = "03 (Mart)" });
            result.Add(new SelectListItem() { Value = "04", Text = "04 (Nisan)" });
            result.Add(new SelectListItem() { Value = "05", Text = "05 (Mayıs)" });
            result.Add(new SelectListItem() { Value = "06", Text = "06 (Haziran)" });
            result.Add(new SelectListItem() { Value = "07", Text = "07 (Temmuz)" });
            result.Add(new SelectListItem() { Value = "08", Text = "08 (Ağustos)" });
            result.Add(new SelectListItem() { Value = "09", Text = "09 (Eylül)" });
            result.Add(new SelectListItem() { Value = "10", Text = "10 (Ekim)" });
            result.Add(new SelectListItem() { Value = "11", Text = "11 (Kasım)" });
            result.Add(new SelectListItem() { Value = "12", Text = "12 (Aralık)" });

            return result;
        }

        private IEnumerable<SelectListItem> GetYears()
        {
            var result = new List<SelectListItem>();

            var year = DateTime.Now.Year;

            result.Add(new SelectListItem() { Value = "", Text = "Yıl" });
            for (int i = 0; i < 10; i++)
            {
                var yearText = year.ToString();
                result.Add(new SelectListItem() { Value = yearText.Substring(2), Text = yearText });
                year++;
            }
            return result;
        }
    }
}