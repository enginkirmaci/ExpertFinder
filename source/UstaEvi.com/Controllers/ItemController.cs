using Common.Entities.Enums;
using Common.Utilities.Converters;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Item;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;
using System;
using System.Threading.Tasks;

namespace UstaEvi.com.Controllers
{
    [Authorize]
    public class ItemController : BaseController
    {
        private readonly UserManager<User> _userManager;
        private readonly IUserEngine _userEngine;
        private readonly INotificationService _notificationService;
        private readonly ILogger _logger;
        private readonly IProjectEngine _projectEngine;

        public ItemController(
            UserManager<User> userManager,
           IUserEngine userEngine,
           IProjectEngine projectEngine,
           INotificationService notificationService,
           ILogger logger,
           IContentEngine contentEngine
           ) : base(contentEngine)
        {
            _userManager = userManager;
            _userEngine = userEngine;
            _projectEngine = projectEngine;
            _notificationService = notificationService;
            _logger = logger;
        }

        [Route("arama/{keyword?}/{zone?}", Name = "search")]
        [HttpGet]
        [AllowAnonymous]
        async public Task<IActionResult> Search(
            string keyword,
            string zone,
            Guid? categoryId = null,
            int? provinceId = null,
            int? districtId = null)
        {
            SetPageContent("search");

            var result = _projectEngine.SearchItems(keyword, zone, categoryId: categoryId, provinceId: provinceId, districtId: districtId);

            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));

            var model = new SearchViewModel()
            {
                PageNumber = 0,
                Result = result,
                SearchSortTypes = _projectEngine.GetSearchSortTypes(),
                SearchSortTypeId = (int)SearchSortTypes.Newest,
                Provinces = _projectEngine.GetProvincesSelectList(),
                Categories = _projectEngine.GetUserCategories()
            };

            if (provinceId.HasValue)
                model.Districts = _projectEngine.GetDistrictsSelectList(provinceId);
            else
                model.Districts = _projectEngine.GetDistrictsSelectList(null);

            return View(model);
        }

        [Route("arama/{model?}", Name = "searchPost")]
        [HttpPost]
        [AllowAnonymous]
        public IActionResult Search(SearchViewModel model)
        {
            SetPageContent("search");

            if (Request.Form.ContainsKey("paging"))
                model.PageNumber = int.Parse(Request.Form["paging"]);

            var result = _projectEngine.SearchItems(model);

            model.Result = result;
            model.SearchSortTypes = _projectEngine.GetSearchSortTypes();
            model.Provinces = _projectEngine.GetProvincesSelectList();
            model.Districts = _projectEngine.GetDistrictsSelectList(model.ProvinceId);
            model.Categories = _projectEngine.GetUserCategories();

            return View(model);
        }

        [Route("hizmetolustur/{categoryslug?}", Name = "createItem")]
        [HttpGet]
        async public Task<IActionResult> CreateItem(string categoryslug, string title)
        {
            SetPageContent("createItem");

            return View(await getCreateItemViewModel(categoryslug, title));
        }

        [Route("hizmetolustur/{categoryslug?}", Name = "createItem")]
        [HttpPost]
        async public Task<IActionResult> CreateItem(string categoryslug, CreateItemViewModel model)
        {
            SetPageContent("createItem");
            if (ModelState.IsValid)
            {
                var questions = _projectEngine.GetCategoryQuestions(categoryslug);

                foreach (var question in questions)
                {
                    if (question.CategoryQuestionTypeId == 3)
                    {
                        var values = HttpContext.Request.Form[Converter.String.ToValidControlId(question.Label)];

                        question.Value = string.Join("|", values);
                    }
                    else
                        question.Value = HttpContext.Request.Form[Converter.String.ToValidControlId(question.Label)];
                }

                var title = HttpContext.Request.Form["Title"];
                var description = HttpContext.Request.Form["Description"];
                var provinceId = HttpContext.Request.Form["ProvinceId"];
                var districtId = HttpContext.Request.Form["DistrictId"];
                var whenTypeId = HttpContext.Request.Form["WhenTypeId"];
                var whenDateId = HttpContext.Request.Form["WhenDateId"];
                var whenTimeId = HttpContext.Request.Form["WhenTimeId"];
                var price = HttpContext.Request.Form["Price"];

                decimal decimalPrice = 0;
                decimal? PriceValue = null;
                if (decimal.TryParse(price, out decimalPrice))
                {
                    PriceValue = decimalPrice;
                }
                if (model.UnknownPrice)
                {
                    PriceValue = null;
                }

                var galleryImages = Request.Form.Files.GetFiles("galleryImage");

                var result = _projectEngine.CreateItem(_userManager.GetUserId(User), categoryslug, title, description, provinceId, districtId,
                    whenTypeId, whenDateId, whenTimeId, questions, PriceValue, galleryImages);

                switch (result.Type)
                {
                    case TransactionType.Error:

                        ModelState.AddModelError("", result.Message);
                        return View(await getCreateItemViewModel(categoryslug));

                    case TransactionType.Success:
                        return RedirectToRoute("projects");
                }
            }

            SetValidationErrorMessage();

            return View(await getCreateItemViewModel(categoryslug));
        }

        private void SetValidationErrorMessage()
        {
            ModelStateEntry entry = null;
            if (ModelState.TryGetValue("Price", out entry) && entry.Errors.Count > 0)
            {
                bool hasPriceError = false;
                foreach (var error in entry.Errors)
                    if (error.ErrorMessage.Contains("Price"))
                    {
                        hasPriceError = true;
                        break;
                    }

                if (hasPriceError)
                {
                    entry.Errors.Clear();
                    entry.Errors.Add("Lütfen bütçeyi doğru giriniz.");
                }
            }

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

            if (ModelState.TryGetValue("WhenTypeId", out entry) && entry.Errors.Count > 0)
            {
                entry.Errors.Clear();
                entry.Errors.Add("Bir zaman seçmelisiniz.");
            }
        }

        [Route("hizmetduzenle/{categoryslug}/{itemslug}", Name = "editItem")]
        [HttpGet]
        public IActionResult EditItem(string categoryslug, string itemslug)
        {
            SetPageContent("createItem");

            var model = _projectEngine.GetEditItemViewModel(_userManager.GetUserId(User), categoryslug, itemslug);

            return View(model);
        }

        [Route("hizmetduzenle/{categoryslug}/{itemslug}", Name = "editItem")]
        [HttpPost]
        async public Task<IActionResult> EditItem(string categoryslug, string itemslug, string cancelItem, EditItemViewModel model)
        {
            // Cancel Item
            if (!string.IsNullOrEmpty(cancelItem))
            {
                var itemId = HttpContext.Request.Form["ItemId"];
                _projectEngine.UpdateItemStatus(new Guid(itemId), StatusTypes.Declined);
                return RedirectToRoute("projects");
            }

            SetPageContent("createItem");
            if (ModelState.IsValid)
            {
                var questions = _projectEngine.GetCategoryQuestions(categoryslug);

                foreach (var question in questions)
                {
                    if (question.CategoryQuestionTypeId == 3)
                    {
                        var values = HttpContext.Request.Form[Converter.String.ToValidControlId(question.Label)];

                        question.Value = string.Join("|", values);
                    }
                    else
                        question.Value = HttpContext.Request.Form[Converter.String.ToValidControlId(question.Label)];
                }

                var itemId = HttpContext.Request.Form["ItemId"];
                var title = HttpContext.Request.Form["Title"];
                var description = HttpContext.Request.Form["Description"];
                var provinceId = HttpContext.Request.Form["ProvinceId"];
                var districtId = HttpContext.Request.Form["DistrictId"];
                var whenTypeId = HttpContext.Request.Form["WhenTypeId"];
                var whenDateId = HttpContext.Request.Form["WhenDateId"];
                var whenTimeId = HttpContext.Request.Form["WhenTimeId"];
                var price = HttpContext.Request.Form["Price"];

                decimal decimalPrice = 0;
                decimal? PriceValue = null;
                if (decimal.TryParse(price, out decimalPrice))
                {
                    PriceValue = decimalPrice;
                }

                if (model.UnknownPrice)
                {
                    PriceValue = null;
                }

                var galleryImages = Request.Form.Files.GetFiles("galleryImage");

                var result = _projectEngine.EditItem(_userManager.GetUserId(User), new Guid(itemId), categoryslug, title, description, provinceId, districtId,
                    whenTypeId, whenDateId, whenTimeId, questions, PriceValue, galleryImages);

                switch (result.Type)
                {
                    case TransactionType.Error:

                        ModelState.AddModelError("", result.Message);
                        return View(await getEditItemViewModel(categoryslug));

                    case TransactionType.Success:
                        return RedirectToRoute("projects");
                }
            }

            SetValidationErrorMessage();

            return View(await getEditItemViewModel(categoryslug));
        }

        [Route("{category}/{slugurl}", Name = "viewItem")]
        [HttpGet]
        [AllowAnonymous]
        public IActionResult ViewItem(string category, string slugurl)
        {
            var model = _projectEngine.GetItem(_userManager.GetUserId(User), category, slugurl);

            ViewBag.Title = model.Item.Title;

            return View(model);
        }

        [Route("{category}/{slugurl}", Name = "viewItem")]
        [HttpPost]
        public IActionResult ViewItem(string category, string slugurl, ViewItemViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = _projectEngine.CreateOffer(_userManager.GetUserId(User), model.Item.Id, category, slugurl, model.Comment, model.OfferPrice.Value);

                switch (result.Type)
                {
                    case TransactionType.Success:
                        return RedirectToRoute("viewItem", new { category = category, slugurl = slugurl });

                    case TransactionType.Error:
                        model = _projectEngine.GetItem(_userManager.GetUserId(User), category, slugurl);
                        ViewBag.Title = model.Item.Title;

                        ModelState.AddModelError("", result.Message);
                        return View(model);
                }
            }

            return View(model);
        }

        [Route("{category}/{slugurl}/kabulet", Name = "acceptoffer")]
        [HttpPost]
        public IActionResult ViewItem(string category, string slugurl, string acceptButton, string rateButton, string rateValue, string rateComment)
        {
            if (!string.IsNullOrWhiteSpace(rateButton))
            {
                var result = _projectEngine.CommentOffer(rateButton, rateValue, rateComment);

                if (result.Type == TransactionType.Success)
                    return RedirectToRoute("viewItem", new { category = category, slugurl = slugurl });
                else
                {
                    var model = _projectEngine.GetItem(_userManager.GetUserId(User), category, slugurl);
                    ViewBag.Title = model.Item.Title;

                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }

            if (!string.IsNullOrWhiteSpace(acceptButton))
            {
                var result = _projectEngine.AcceptOffer(_userManager.GetUserId(User), acceptButton);

                if (result.Type == TransactionType.Success)
                    return RedirectToRoute("viewItem", new { category = category, slugurl = slugurl });
                else
                {
                    var model = _projectEngine.GetItem(_userManager.GetUserId(User), category, slugurl);
                    ViewBag.Title = model.Item.Title;

                    ModelState.AddModelError("", result.Message);
                    return View(model);
                }
            }

            return RedirectToRoute("viewItem", new { category = category, slugurl = slugurl });
        }

        private async Task<CreateItemViewModel> getCreateItemViewModel(string categoryslug, string titleStr = null)
        {
            var user = await _userEngine.CurrentUser(_userManager.GetUserId(User));
            var questions = _projectEngine.GetCategoryQuestions(categoryslug);

            StringValues title;
            StringValues description;
            StringValues provinceId;
            StringValues districtId;
            StringValues whenTypeId;
            StringValues whenDateId;
            StringValues whenTimeId;
            StringValues price;

            if (HttpContext.Request.HasFormContentType)
            {
                title = HttpContext.Request.Form["Title"];
                description = HttpContext.Request.Form["Description"];
                provinceId = HttpContext.Request.Form["ProvinceId"];
                districtId = HttpContext.Request.Form["DistrictId"];
                whenTypeId = HttpContext.Request.Form["WhenTypeId"];
                whenDateId = HttpContext.Request.Form["WhenDateId"];
                whenTimeId = HttpContext.Request.Form["WhenTimeId"];
                price = HttpContext.Request.Form["Price"];

                foreach (var question in questions)
                {
                    question.Value = HttpContext.Request.Form[Converter.String.ToValidControlId(question.Label)];
                }
            }

            var model = new CreateItemViewModel()
            {
                Title = !string.IsNullOrWhiteSpace(titleStr) ? titleStr : title.ToString(),
                Description = description,
                SelectedCategory = categoryslug,
                Categories = _projectEngine.GetUserCategories(true),
                Questions = questions,
                Provinces = _projectEngine.GetProvincesSelectList(),
                ProvinceId = provinceId == string.Empty ? TryParse(provinceId) : (user.ProvinceId.HasValue ? user.ProvinceId.Value : 0),
                Districts = _projectEngine.GetDistrictsSelectList(provinceId == string.Empty || provinceId.Count == 0 ? user.ProvinceId : TryParse(provinceId)),
                //DistrictId = districtId == string.Empty ? TryParse(districtId) : (user.DistrictId.HasValue ? user.DistrictId.Value : 0),
                DistrictId = districtId == string.Empty ? TryParse(districtId) : 0,
                WhenTypes = _projectEngine.GetWhenTypes(),
                WhenTypeId = whenTypeId == string.Empty ? TryParse(whenTypeId) : 0,
                WhenDates = _projectEngine.GetWhenDates(),
                WhenDateId = whenDateId,
                WhenTimes = _projectEngine.GetWhenTimes(),
                WhenTimeId = whenTimeId
            };

            decimal decimalPrice = 0;
            decimal? PriceValue = null;
            if (decimal.TryParse(price, out decimalPrice))
            {
                PriceValue = decimalPrice;
            }
            model.Price = PriceValue;

            return model;
        }

        private int TryParse(StringValues values, int defaultValue = 0)
        {
            var result = defaultValue;
            int.TryParse(values, out result);
            return result;
        }

        private async Task<EditItemViewModel> getEditItemViewModel(string categoryslug)
        {
            var model = await getCreateItemViewModel(categoryslug);

            StringValues itemId;

            if (HttpContext.Request.HasFormContentType)
            {
                itemId = HttpContext.Request.Form["ItemId"];
            }

            return new EditItemViewModel()
            {
                ItemId = new Guid(itemId),
                Title = model.Title,
                Description = model.Description,
                SelectedCategory = model.SelectedCategory,
                Categories = model.Categories,
                Questions = model.Questions,
                Provinces = model.Provinces,
                ProvinceId = model.ProvinceId,
                Districts = model.Districts,
                DistrictId = model.DistrictId,
                WhenTypes = model.WhenTypes,
                WhenTypeId = model.WhenTypeId,
                WhenDates = model.WhenDates,
                WhenDateId = model.WhenDateId,
                WhenTimes = model.WhenTimes,
                WhenTimeId = model.WhenTimeId,
                Price = model.Price
            };
        }
    }
}