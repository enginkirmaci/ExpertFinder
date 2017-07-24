using Common.Entities.Enums;
using Common.Utilities.Converters;
using Common.Web.Data;
using Common.Web.IO;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Converters;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.Helper;
using ExpertFinder.Common.ViewModels.Item;
using ExpertFinder.Common.ViewModels.Manage;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using TransactionResult = Common.Entities.TransactionResult;

namespace ExpertFinder.Application
{
    public class ProjectEngine : IProjectEngine
    {
        private const int PAGEITEMCOUNT = 10;
        private readonly teklifcepteDBContext _dbContext;
        private readonly ICachingEx _cachingEx;
        private readonly INotificationService _notificationService;
        private readonly IUserEngine _userEngine;
        private readonly ICategoryServices _categoryServices;
        private readonly IAdminEngine _adminEngine;
        private readonly IConfiguration _configuration;
        private readonly IHostingEnvironment _environment;

        public ProjectEngine(
            teklifcepteDBContext dbContext,
            ICachingEx cachingEx,
            INotificationService notificationService,
            IUserEngine userEngine,
            ICategoryServices categoryServices,
            IAdminEngine adminEngine,
            IConfiguration configuration,
            IHostingEnvironment environment
            )
        {
            _dbContext = dbContext;
            _cachingEx = cachingEx;
            _notificationService = notificationService;
            _userEngine = userEngine;
            _categoryServices = categoryServices;
            _adminEngine = adminEngine;
            _configuration = configuration;
            _environment = environment;
        }

        private void AddImagesToList(List<string> images, string image)
        {
            if (!string.IsNullOrWhiteSpace(image))
                images.Add(image);
        }

        public EditItemViewModel GetEditItemViewModel(string UserId, string categorySlug, string itemSlug)
        {
            var item = _dbContext.Item
                .Include(i => i.Province)
                .Include(i => i.District)
                .Include(i => i.User.Province)
                .Include(i => i.User.District)
                .Include(i => i.Category)
                .Include(i => i.Category.Parent)
                .FirstOrDefault(i => i.Category.SlugUrl == categorySlug && i.SlugUrl == itemSlug && i.UserId == UserId);

            if (item != null)
            {
                var questions = JsonConvert.DeserializeObject<IEnumerable<CategoryQuestions>>(item.ItemCategoryQuestionsJSON);

                var images = new List<string>();
                AddImagesToList(images, item.ImageUrl);
                AddImagesToList(images, item.ImageUrl2);
                AddImagesToList(images, item.ImageUrl3);
                AddImagesToList(images, item.ImageUrl4);
                AddImagesToList(images, item.ImageUrl5);

                var model = new EditItemViewModel()
                {
                    ItemId = item.Id,
                    Title = item.Title,
                    Description = item.Description,
                    SelectedCategory = categorySlug,
                    Categories = GetUserCategories(true),
                    Questions = questions,
                    Provinces = GetProvincesSelectList(),
                    ProvinceId = item.ProvinceId.HasValue ? item.ProvinceId.Value : 0,
                    Districts = GetDistrictsSelectList(item.ProvinceId),
                    DistrictId = item.DistrictId.HasValue ? item.DistrictId.Value : 0,
                    WhenTypes = GetWhenTypes(),
                    WhenTypeId = item.WhenType,
                    WhenDates = GetWhenDates(),
                    WhenDateId = item.WhenDate.ToString("yyyyMMdd"),
                    WhenTimes = GetWhenTimes(),
                    WhenTimeId = item.WhenDate.ToString("HHmm"),
                    Price = item.Price,
                    Images = images
                };

                return model;
            }

            return null;
        }

        public Item GetItem(Guid Id)
        {
            var item = _dbContext.Item
                .Include(i => i.Category)
                .FirstOrDefault(i => i.Id == Id);

            return item;
        }

        public ViewItemViewModel GetItem(string UserId, string categorySlug, string itemSlug)
        {
            var item = _dbContext.Item
                .Include(i => i.Province)
                .Include(i => i.District)
                .Include(i => i.User.Province)
                .Include(i => i.User.District)
                .Include(i => i.Category)
                .Include(i => i.Category.Parent)
                .FirstOrDefault(i => i.Category.SlugUrl == categorySlug && i.SlugUrl == itemSlug);

            var questions = JsonConvert.DeserializeObject<IEnumerable<CategoryQuestions>>(item.ItemCategoryQuestionsJSON);

            var images = new List<string>();
            AddImagesToList(images, item.ImageUrl);
            AddImagesToList(images, item.ImageUrl2);
            AddImagesToList(images, item.ImageUrl3);
            AddImagesToList(images, item.ImageUrl4);
            AddImagesToList(images, item.ImageUrl5);

            var model = new ViewItemViewModel()
            {
                ItemUser = item.User,
                Item = item,
                Questions = questions,
                Images = images
            };

            if (item.UserId == UserId)
            {
                model.Offers = _dbContext.Offer
                    .Include(i => i.User)
                    .Where(i => i.ItemId == item.Id)
                    .OrderByDescending(i => i.OfferDate);

                var winner = model.Offers.FirstOrDefault(i => i.IsWinner == true);
                if (winner != null)
                {
                    model.hasWinner = true;
                    model.Offers = new List<Offer>()
                    {
                        winner
                    };
                }
            }
            else
            {
                var offer = _dbContext.Offer
                    .Include(i => i.User)
                    .FirstOrDefault(i => i.ItemId == item.Id && i.UserId == UserId);

                model.Offers = new List<Offer>();

                if (offer != null)
                {
                    if (offer.IsWinner.HasValue && offer.IsWinner.Value)
                        model.hasWinner = true;

                    model.Offers = new List<Offer>()
                    {
                        offer
                    };
                }
            }

            return model;
        }

        public TransactionResult CreateOffer(string UserId, Guid itemId, string categorySlug, string itemSlug, string comment, decimal price)
        {
            try
            {
                var tokenCount = _userEngine.CurrentUserToken(UserId, disableCache: true);
                if (tokenCount <= 0)
                    return new TransactionResult(message: "Yeterli jetonunuz bulunmamaktadır. Teklif verebilmek için jeton almanız gerekmektedir.", type: TransactionType.Error);

                var myOffer = _dbContext.Offer.FirstOrDefault(i => i.ItemId == itemId && i.UserId == UserId);
                if (myOffer != null)
                    return new TransactionResult(message: "Bu hizmet için verilmiş teklifiniz var. Birden fazla teklif veremezsiniz!", type: TransactionType.Error);

                var item = _dbContext.Item.FirstOrDefault(i => i.Id == itemId);
                if (item != null && (item.StatusID == (int)StatusTypes.Declined || item.StatusID == (int)StatusTypes.Closed))
                    return new TransactionResult(message: "Bu hizmet sonlandığı için teklif veremezsiniz!", type: TransactionType.Error);

                var offer = new Offer();
                offer.Id = Guid.NewGuid();
                offer.UserId = UserId;
                offer.ItemId = itemId;
                offer.OfferPrice = price;
                offer.OfferDate = DateTime.Now;
                offer.Comment = comment;

                _dbContext.Offer.Add(offer);

                var spent = new SpentHistory()
                {
                    Id = Guid.NewGuid(),
                    TokenSpent = 1,
                    UserId = UserId,
                    CreatedDate = DateTime.Now,
                    Title = "Projeye teklif verdiniz."
                };
                _dbContext.SpentHistory.Add(spent);

                _dbContext.SaveChanges();

                _notificationService.AddNotification(UserId, NotificationTypes.CreateOffer, itemId: itemId.ToString());
                _notificationService.AddNotification(item.UserId, NotificationTypes.ReceivedOffer, itemId: itemId.ToString());

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public TransactionResult AcceptOffer(string UserId, string offerId)
        {
            try
            {
                var offer = _dbContext.Offer
                    .Include(i => i.Item)
                    .FirstOrDefault(i => i.Id == new Guid(offerId));

                if (offer != null)
                {
                    offer.IsWinner = true;

                    var item = offer.Item;
                    item.StatusID = (int)StatusTypes.Closed;

                    _dbContext.SaveChanges();

                    _notificationService.AddNotification(UserId, NotificationTypes.AcceptOffer, itemId: item.Id.ToString());
                    _notificationService.AddNotification(offer.UserId, NotificationTypes.ReceivedAcceptOffer, itemId: item.Id.ToString());
                }
                else
                    return new TransactionResult(message: "Teklif bulunamadı. Lütfen tekrar deneyiniz.", type: TransactionType.Error);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        private int getRateValue(string rateValue)
        {
            var result = 1;

            if (int.TryParse(rateValue, out result))
            {
                result = result % 6;

                if (result == 0)
                    result = 1;
            }

            return result;
        }

        private void calculateUserRating(string UserId)
        {
            var User = _dbContext.Users.FirstOrDefault(i => i.Id == UserId);

            if (User != null)
            {
                var ratings = _dbContext.UserRatings.Where(i => i.UserId == UserId).Select(i => i.RateValue);

                if (ratings.Count() > 0)
                {
                    var rateValue = 0.0;

                    rateValue = ratings.Sum() / ratings.Count();

                    User.Rating = rateValue;

                    _dbContext.SaveChanges();
                }
            }
        }

        public TransactionResult CommentOffer(string offerId, string rateValue, string rateComment)
        {
            try
            {
                var offer = _dbContext.Offer
                    .Include(i => i.Item)
                    .FirstOrDefault(i => i.Id == new Guid(offerId));

                if (offer != null)
                {
                    offer.IsRated = true;

                    var itemUserRating = new UserRatings()
                    {
                        Id = Guid.NewGuid(),
                        ItemId = offer.ItemId,
                        WinnerOfferId = offer.Id,
                        UserId = offer.UserId,
                        Comment = rateComment,
                        RateValue = getRateValue(rateValue)
                    };

                    _dbContext.UserRatings.Add(itemUserRating);
                    _dbContext.SaveChanges();

                    calculateUserRating(offer.UserId);
                }
                else
                    return new TransactionResult(message: "Teklif bulunamadı. Lütfen tekrar deneyiniz.", type: TransactionType.Error);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public TransactionResult CommentItem(string itemId, string rateValue, string rateComment)
        {
            try
            {
                var item = _dbContext.Item
                    .FirstOrDefault(i => i.Id == new Guid(itemId));

                if (item != null)
                {
                    item.IsRated = true;

                    var itemUserRating = new UserRatings()
                    {
                        Id = Guid.NewGuid(),
                        ItemId = item.Id,
                        UserId = item.UserId,
                        Comment = rateComment,
                        RateValue = getRateValue(rateValue)
                    };

                    _dbContext.UserRatings.Add(itemUserRating);
                    _dbContext.SaveChanges();

                    calculateUserRating(item.UserId);
                }
                else
                    return new TransactionResult(message: "Hizmet bulunamadı. Lütfen tekrar deneyiniz.", type: TransactionType.Error);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public TransactionResult CreateItem(string UserId, string categoryslug, string title, string description, string provinceId, string districtId, string whenTypeId, string whenDateId, string whenTimeId, IEnumerable<CategoryQuestions> questions, decimal? price, IEnumerable<IFormFile> galleryImages)
        {
            try
            {
                var today = DateTime.Now;

                var item = new Item();
                item.Id = Guid.NewGuid();
                item.AddedDate = today;
                item.SlugUrl = UrlConverter.ToUrlSlug(title + "-" + today.DayOfYear + today.Year);
                var category = _categoryServices.GetCategory(categoryslug);
                item.CategoryId = category.Id;
                item.Description = description;

                int ProvinceId = 0;
                if (int.TryParse(provinceId, out ProvinceId))
                {
                    item.ProvinceId = ProvinceId;
                }

                int DistrictId = 0;
                if (int.TryParse(districtId, out DistrictId))
                {
                    item.DistrictId = DistrictId;
                }

                int WhenTypeId = 0;
                if (int.TryParse(whenTypeId, out WhenTypeId))
                {
                    item.WhenType = WhenTypeId;
                }

                if (!string.IsNullOrWhiteSpace(whenDateId) && !string.IsNullOrWhiteSpace(whenTimeId))
                {
                    var datetime = DateTime.ParseExact(whenDateId + " " + whenTimeId, "yyyyMMdd HHmm", CultureInfo.InvariantCulture);
                    item.WhenDate = datetime;
                }

                item.Price = price;
                item.StatusID = (int)StatusTypes.Open;
                item.Title = title;
                item.UserId = UserId;
                item.ItemCategoryQuestionsJSON = JsonConvert.SerializeObject(questions);

                //// TODO Uncomment here
                //var upload = new FormUpload(_environment.WebRootPath);
                //var index = 0;
                //foreach (var image in galleryImages)
                //{
                //    if (image.Length != 0)
                //    {
                //        var filename = string.Format("{0}-{1}", item.Id, index);
                //        var imageResult = upload.SaveFile(image, filename, "itemgallery");

                //        if (imageResult.Type != TransactionType.Success)
                //            return imageResult;

                //        switch (index)
                //        {
                //            case 0:
                //                item.ImageUrl = imageResult.Result as string;
                //                break;

                //            case 1:
                //                item.ImageUrl2 = imageResult.Result as string;
                //                break;

                //            case 2:
                //                item.ImageUrl3 = imageResult.Result as string;
                //                break;

                //            case 3:
                //                item.ImageUrl4 = imageResult.Result as string;
                //                break;

                //            case 4:
                //                item.ImageUrl5 = imageResult.Result as string;
                //                break;
                //        }

                //        index++;
                //    }
                //}

                _dbContext.Item.Add(item);

                _dbContext.SaveChanges();
                _notificationService.AddNotification(UserId, NotificationTypes.CreateItem, itemId: item.Id.ToString());

                SendBulkSMS(item);

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        private void SendBulkSMS(Item item)
        {
            List<User> userList = _adminEngine.GetUserForSMS(item.ProvinceId, item.DistrictId, item.CategoryId.ToString(), null).ToList();

            userList.RemoveAll(i => i.Id == item.UserId);

            if (userList != null && userList.Capacity > 0)
            {
                List<string> phoneList = new List<string>();
                foreach (var listItem in userList)
                {
                    if (listItem.PhoneNumber != null && !string.IsNullOrEmpty(listItem.PhoneNumber))
                        phoneList.Add(listItem.PhoneNumber);
                }

                if (phoneList.Capacity > 0)
                {
                    Category category = _adminEngine.GetCategory((Guid)item.CategoryId);
                    var url = string.Format("{0}{1}/{2}", _configuration["Frontend:Domain"], category.SlugUrl, item.SlugUrl);
                    SmsHelper.SendBulkSMS(phoneList, string.Format("Yeni bir hizmet geldi. Teklif vermek için {0}", url));
                }
            }
        }

        public TransactionResult EditItem(string UserId, Guid itemId, string categoryslug, string title, string description, string provinceId, string districtId, string whenTypeId, string whenDateId, string whenTimeId, IEnumerable<CategoryQuestions> questions, decimal? price, IEnumerable<IFormFile> galleryImages)
        {
            try
            {
                var item = _dbContext.Item
                    .FirstOrDefault(i => i.Id == itemId);

                if (item != null)
                {
                    var today = DateTime.Now;

                    item.Id = itemId;
                    item.UpdateDate = today;
                    var category = _categoryServices.GetCategory(categoryslug);
                    item.CategoryId = category.Id;
                    item.Description = description;

                    int ProvinceId = 0;
                    if (int.TryParse(provinceId, out ProvinceId))
                    {
                        item.ProvinceId = ProvinceId;
                    }

                    int DistrictId = 0;
                    if (int.TryParse(districtId, out DistrictId))
                    {
                        item.DistrictId = DistrictId;
                    }

                    int WhenTypeId = 0;
                    if (int.TryParse(whenTypeId, out WhenTypeId))
                    {
                        item.WhenType = WhenTypeId;
                    }

                    if (!string.IsNullOrWhiteSpace(whenDateId) && !string.IsNullOrWhiteSpace(whenTimeId))
                    {
                        var datetime = DateTime.ParseExact(whenDateId + " " + whenTimeId, "yyyyMMdd HHmm", CultureInfo.InvariantCulture);
                        item.WhenDate = datetime;
                    }

                    item.Price = price;
                    item.Title = title;
                    item.ItemCategoryQuestionsJSON = JsonConvert.SerializeObject(questions);

                    var upload = new FormUpload(_environment.WebRootPath);
                    var index = 0;

                    if (index < 5)
                        foreach (var image in galleryImages)
                        {
                            if (image.Length != 0)
                            {
                                if (!string.IsNullOrWhiteSpace(item.ImageUrl) && index == 0)
                                    index++;

                                if (!string.IsNullOrWhiteSpace(item.ImageUrl2) && index == 1)
                                    index++;

                                if (!string.IsNullOrWhiteSpace(item.ImageUrl3) && index == 2)
                                    index++;

                                if (!string.IsNullOrWhiteSpace(item.ImageUrl4) && index == 3)
                                    index++;

                                if (!string.IsNullOrWhiteSpace(item.ImageUrl5) && index == 4)
                                    index++;

                                //TODO Uncomment here
                                //var filename = string.Format("{0}-{1}", item.Id, index);
                                //var imageResult = upload.SaveFile(image, filename, "itemgallery");

                                //if (imageResult.Type != TransactionType.Success)
                                //    return imageResult;

                                //switch (index)
                                //{
                                //    case 0:
                                //        item.ImageUrl = imageResult.Result as string;
                                //        break;

                                //    case 1:
                                //        item.ImageUrl2 = imageResult.Result as string;
                                //        break;

                                //    case 2:
                                //        item.ImageUrl3 = imageResult.Result as string;
                                //        break;

                                //    case 3:
                                //        item.ImageUrl4 = imageResult.Result as string;
                                //        break;

                                //    case 4:
                                //        item.ImageUrl5 = imageResult.Result as string;
                                //        break;
                                //}

                                index++;
                            }
                        }

                    _dbContext.SaveChanges();
                    _notificationService.AddNotification(UserId, NotificationTypes.EditItem, itemId: item.Id.ToString());

                    return new TransactionResult() { Type = TransactionType.Success };
                }
                else
                    return new TransactionResult(message: "Hizmetiniz sistemimizde bulunamıyor. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public TransactionResult RemoveItemGallery(string userID, Guid itemId, string url)
        {
            try
            {
                var item = _dbContext.Item.FirstOrDefault(i => i.Id == itemId && i.UserId == userID);

                if (item != null)
                {
                    if (item.ImageUrl == url)
                        item.ImageUrl = null;
                    else if (item.ImageUrl2 == url)
                        item.ImageUrl2 = null;
                    else if (item.ImageUrl3 == url)
                        item.ImageUrl3 = null;
                    else if (item.ImageUrl4 == url)
                        item.ImageUrl4 = null;
                    else if (item.ImageUrl5 == url)
                        item.ImageUrl5 = null;

                    _dbContext.SaveChanges();

                    var upload = new FormUpload(_environment.WebRootPath);
                    upload.RemoveFile(url, "itemgallery");
                }

                return new TransactionResult(type: TransactionType.Success);
            }
            catch
            {
                return new TransactionResult(message: "Beklenmedik bir hata oluştu. Lütfen tekrar deneyiniz.", type: TransactionType.Error);
            }
        }

        public IEnumerable<Item> SearchItems(SearchViewModel model)
        {
            return SearchItems(
                model.Keyword,
                null,
                page: model.PageNumber,
                sortOrder: (SearchSortTypes)model.SearchSortTypeId,
                categories: model.SelectedCategories,
                provinceId: model.ProvinceId,
                districtId: model.DistrictId
                );
        }

        private static CultureInfo turkish = new CultureInfo("tr-TR");

        public IEnumerable<Item> SearchItems(
            string keyword,
            string zone,
            Guid? categoryId = null,
            int? provinceId = null,
            int? districtId = null,
            SearchSortTypes sortOrder = SearchSortTypes.Newest,
            IEnumerable<string> categories = null,
            int page = 0)
        {
            var result = _dbContext.Item
                .Include(i => i.District)
                .Include(i => i.Province)
                .Include(i => i.Category)
                .Include(i => i.User)
                .Where(i => i.StatusID == (int)StatusTypes.Open);

            if (categoryId != null)
                result = result.Where(i => i.CategoryId == categoryId);

            if (provinceId != null)
                result = result.Where(i => i.ProvinceId == provinceId);

            if (districtId != null)
                result = result.Where(i => i.DistrictId == districtId);

            if (!string.IsNullOrWhiteSpace(keyword))
                result = result.Where(i => i.Title.ToLower(turkish).Contains(keyword.ToLower(turkish)));

            if (!string.IsNullOrWhiteSpace(zone))
            {
                var province = GetProvincesSelectList().FirstOrDefault(i => i.Text == zone);
                if (province != null)
                    result = result.Where(i => i.ProvinceId == int.Parse(province.Value));
                else
                {
                    var district = _dbContext.District.FirstOrDefault(i => i.Name == zone);
                    if (district != null)
                        result = result.Where(i => i.DistrictId == district.Id);
                }
            }

            IEnumerable<Item> listResult = null;

            if (categories != null)
            {
                var filteredByCategory = new List<Item>();
                foreach (var item in categories)
                    filteredByCategory.AddRange(result.Where(i => item == i.Category.SlugUrl).ToList());

                listResult = filteredByCategory.Distinct();
            }
            else
                listResult = result.ToList();

            switch (sortOrder)
            {
                case SearchSortTypes.Newest:
                    listResult = listResult.OrderByDescending(i => i.AddedDate);
                    break;

                case SearchSortTypes.Oldest:
                    listResult = listResult.OrderBy(i => i.AddedDate);
                    break;

                case SearchSortTypes.HighRate:
                    listResult = listResult.OrderByDescending(i => i.Price);
                    break;

                case SearchSortTypes.LowRate:
                    listResult = listResult.OrderBy(i => i.Price);
                    break;
            }

            listResult = listResult
                .Skip(page * PAGEITEMCOUNT)
                .Take(PAGEITEMCOUNT);

            return listResult;
        }

        public IEnumerable<Item> GetUserItems(string UserId, ProjectFilterTypes FilterType)
        {
            var result = _dbContext.Item
                .Include(i => i.District)
                .Include(i => i.Province)
                .Include(i => i.Category)
                .Include(i => i.Offer)
                .Where(i => i.UserId == UserId);

            switch (FilterType)
            {
                case ProjectFilterTypes.Open:
                    result = result.Where(i => i.StatusID == (int)StatusTypes.Open);
                    break;

                case ProjectFilterTypes.Closed:
                    result = result.Where(i => i.StatusID == (int)StatusTypes.Closed);
                    break;

                case ProjectFilterTypes.Declined:
                    result = result.Where(i => i.StatusID == (int)StatusTypes.Declined);
                    break;
            }

            return result.OrderByDescending(i => i.AddedDate);
        }

        public IEnumerable<Offer> GetUserOffers(string UserId, OfferFilterTypes FilterType)
        {
            var result = _dbContext.Offer
                .Include(i => i.Item)
                .Include(i => i.Item.User)
                .Include(i => i.Item.Category)
                .Include(i => i.Item.Category.Parent)
                .Where(i => i.UserId == UserId);

            switch (FilterType)
            {
                case OfferFilterTypes.Open:
                    result = result.Where(i => i.Item.StatusID == (int)StatusTypes.Open);
                    break;

                case OfferFilterTypes.Closed:
                    result = result.Where(i => (!i.IsWinner.HasValue || i.IsWinner == false) && i.Item.StatusID != (int)StatusTypes.Open);
                    break;

                case OfferFilterTypes.Winner:
                    result = result.Where(i => (i.IsWinner.HasValue && i.IsWinner.Value));
                    break;
            }

            return result.OrderByDescending(i => i.OfferDate);
        }

        public IEnumerable<TokenItem> GetUserTokens(string UserId)
        {
            var bought = _dbContext.BoughtHistory
                .Where(i => i.UserId == UserId && i.IsPaid).Select(i => new TokenItem
                {
                    Title = i.Campain.Title,
                    Count = i.Campain.TokenCount,
                    Date = i.CreatedDate
                })
                .ToList();

            var spent = _dbContext.SpentHistory
                .Where(i => i.UserId == UserId).Select(i => new TokenItem
                {
                    Title = i.Title,
                    Count = -i.TokenSpent,
                    Date = i.CreatedDate
                })
                .ToList();

            bought.AddRange(spent);

            return bought.OrderByDescending(i => i.Date);
        }

        public IEnumerable<string> GetProvinceDistrict()
        {
            if (!_cachingEx.Exists("ProvinceDistrict"))
            {
                var provinces = _dbContext.Province
                    .OrderBy(i => i.Name)
                    .Select(i => i.Name)
                    .ToList();

                var districts = _dbContext.District
                    .Select(i => i.Name)
                    .ToList();

                provinces.AddRange(districts);

                _cachingEx.Add("ProvinceDistrict", provinces);
            }

            return _cachingEx.Get<List<string>>("ProvinceDistrict");
        }

        public IEnumerable<Province> GetProvinces()
        {
            if (!_cachingEx.Exists("Provinces"))
            {
                var provinces = _dbContext.Province
                    .Include(i => i.District)
                    .OrderBy(i => i.Name)
                    .ToList();

                _cachingEx.Add("Provinces", provinces);
            }

            return _cachingEx.Get<List<Province>>("Provinces");
        }

        public IEnumerable<SelectListItem> GetProvincesSelectList()
        {
            if (!_cachingEx.Exists("ProvincesSelectList"))
            {
                var provinces = GetProvinces()
                    .OrderBy(i => i.Priority)
                    .Select(i => new SelectListItem()
                    {
                        Text = i.Name,
                        Value = i.Id.ToString()
                    })
                    .ToList();

                provinces.Insert(0, new SelectListItem() { Value = "", Text = "İl seçiniz..." });

                _cachingEx.Add("ProvincesSelectList", provinces);
            }

            return _cachingEx.Get<List<SelectListItem>>("ProvincesSelectList");
        }

        public IEnumerable<SelectListItem> GetDistrictsSelectList(int? provinceId)
        {
            var districts = new List<SelectListItem>();

            if (provinceId.HasValue)
            {
                districts = _dbContext.District
                  .Where(i => i.ProvinceId == provinceId.Value)
                  .OrderBy(i => i.Name)
                  .Select(i => new SelectListItem()
                  {
                      Text = i.Name,
                      Value = i.Id.ToString()
                  })
                  .ToList();
            }

            districts.Insert(0, new SelectListItem() { Value = "", Text = "İlçe seçiniz..." });

            return districts;
        }

        public IEnumerable<SelectListItem> GetUserCategories(bool addEmpty = false)
        {
            var result = _categoryServices.GetCategories()
               .Select(i => i.Parent != null ? new SelectListItem() { Value = i.SlugUrl, Text = i.Parent.Name + " ˃ " + i.Name } : new SelectListItem() { Value = i.SlugUrl, Text = i.Name })
               .OrderBy(i => i.Text)
               .ToList();

            if (addEmpty)
                result.Insert(0, new SelectListItem() { Value = "", Text = "Kategori seçiniz..." }); //, Text = "Kategori seçiniz..." });

            return result;
        }

        public IEnumerable<CategoryQuestions> GetCategoryQuestions(string categorySlug)
        {
            if (string.IsNullOrEmpty(categorySlug))
                return null;

            var category = _categoryServices.GetCategory(categorySlug);

            if (category == null)
                return null;

            var questions = _dbContext.CategoryQuestions
                 .Where(i => i.CategoryId == category.Id)
                 .OrderBy(i => i.Priority)
                 .ToList();

            return questions;
        }

        public IEnumerable<SelectListItem> GetWhenTypes()
        {
            if (!_cachingEx.Exists("WhenTypes"))
            {
                var result = new List<SelectListItem>();

                result.Add(new SelectListItem() { Value = "", Text = "Zaman seçiniz..." });
                result.Add(new SelectListItem() { Value = ((int)WhenType.SpecificDate).ToString(), Text = "Belirli bir zaman (İki hafta içinde)" });
                result.Add(new SelectListItem() { Value = ((int)WhenType.OneMonth).ToString(), Text = "Bir ay içinde" });
                result.Add(new SelectListItem() { Value = ((int)WhenType.ThreeMonth).ToString(), Text = "Üç ay içinde" });
                result.Add(new SelectListItem() { Value = ((int)WhenType.SixMonth).ToString(), Text = "Altı ay içinde" });

                _cachingEx.Add("WhenTypes", result);
            }

            return _cachingEx.Get<List<SelectListItem>>("WhenTypes");
        }

        public IEnumerable<SelectListItem> GetWhenDates()
        {
            var result = new List<SelectListItem>();

            var beginDate = DateTime.Now;

            for (int i = 1; i <= 14; i++)
            {
                result.Add(new SelectListItem() { Value = beginDate.ToString("yyyyMMdd"), Text = beginDate.ToDayMonthDayname() });
                beginDate = beginDate.AddDays(1);
            }

            return result;
        }

        public IEnumerable<SelectListItem> GetWhenTimes()
        {
            if (!_cachingEx.Exists("WhenTimes"))
            {
                var result = new List<SelectListItem>();

                var beginDate = new DateTime(2000, 1, 1, 0, 0, 0);
                for (int i = 1; i <= 48; i++)
                {
                    result.Add(new SelectListItem() { Value = beginDate.ToString("HHmm"), Text = beginDate.ToHourMinute() });
                    beginDate = beginDate.AddMinutes(30);
                }

                _cachingEx.Add("WhenTimes", result);
            }

            return _cachingEx.Get<List<SelectListItem>>("WhenTimes");
        }

        public IEnumerable<SelectListItem> GetSearchSortTypes()
        {
            if (!_cachingEx.Exists("SearchSortTypes"))
            {
                var result = new List<SelectListItem>();

                result.Add(new SelectListItem() { Value = ((int)SearchSortTypes.Newest).ToString(), Text = "Yeniden eskiye" });
                result.Add(new SelectListItem() { Value = ((int)SearchSortTypes.Oldest).ToString(), Text = "Eskiden yeniye" });
                result.Add(new SelectListItem() { Value = ((int)SearchSortTypes.HighRate).ToString(), Text = "Bütçeye göre azalan" });
                result.Add(new SelectListItem() { Value = ((int)SearchSortTypes.LowRate).ToString(), Text = "Bütçeye göre artan" });

                _cachingEx.Add("SearchSortTypes", result);
            }

            return _cachingEx.Get<List<SelectListItem>>("SearchSortTypes");
        }

        public IEnumerable<SelectListItem> GetProjectFilterTypes()
        {
            if (!_cachingEx.Exists("ProjectFilterTypes"))
            {
                var result = new List<SelectListItem>();

                result.Add(new SelectListItem() { Value = ((int)ProjectFilterTypes.All).ToString(), Text = "Hepsi" });
                result.Add(new SelectListItem() { Value = ((int)ProjectFilterTypes.Open).ToString(), Text = "Aktif Hizmetler" });
                result.Add(new SelectListItem() { Value = ((int)ProjectFilterTypes.Closed).ToString(), Text = "Sonlanan Hizmetler" });
                result.Add(new SelectListItem() { Value = ((int)ProjectFilterTypes.Declined).ToString(), Text = "Reddedilenler Hizmetler" });

                _cachingEx.Add("ProjectFilterTypes", result);
            }

            return _cachingEx.Get<List<SelectListItem>>("ProjectFilterTypes");
        }

        public IEnumerable<SelectListItem> GetOfferFilterTypes()
        {
            if (!_cachingEx.Exists("OfferFilterTypes"))
            {
                var result = new List<SelectListItem>();

                result.Add(new SelectListItem() { Value = ((int)OfferFilterTypes.All).ToString(), Text = "Hepsi" });
                result.Add(new SelectListItem() { Value = ((int)OfferFilterTypes.Open).ToString(), Text = "Açık Teklifler" });
                result.Add(new SelectListItem() { Value = ((int)OfferFilterTypes.Winner).ToString(), Text = "Kazanılan Teklifler" });
                result.Add(new SelectListItem() { Value = ((int)OfferFilterTypes.Closed).ToString(), Text = "Kabul Edilmemişler" });

                _cachingEx.Add("OfferFilterTypes", result);
            }

            return _cachingEx.Get<List<SelectListItem>>("OfferFilterTypes");
        }

        public TransactionResult UpdateItemStatus(Guid id, StatusTypes status)
        {
            try
            {
                var item = _dbContext.Item.Where(i => i.Id == id).FirstOrDefault();

                item.StatusID = (int)status;

                _dbContext.SaveChanges();

                return new TransactionResult() { Type = TransactionType.Success };
            }
            catch (Exception ex)
            {
                return new TransactionResult(message: ex.Message, type: TransactionType.Error);
            }
        }
    }
}