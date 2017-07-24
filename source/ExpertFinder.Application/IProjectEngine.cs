using Common.Entities;
using ExpertFinder.Common.Enums;
using ExpertFinder.Common.ViewModels.Item;
using ExpertFinder.Common.ViewModels.Manage;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;

namespace ExpertFinder.Application.Interfaces
{
    public interface IProjectEngine
    {
        EditItemViewModel GetEditItemViewModel(string UserId, string categorySlug, string itemSlug);

        Item GetItem(Guid Id);

        ViewItemViewModel GetItem(string UserId, string categorySlug, string itemSlug);

        TransactionResult AcceptOffer(string UserId, string offerId);

        TransactionResult CommentOffer(string offerId, string rateValue, string rateComment);

        TransactionResult CommentItem(string itemId, string rateValue, string rateComment);

        TransactionResult CreateOffer(string UserId, Guid itemId, string categorySlug, string itemSlug, string comment, decimal price);

        TransactionResult CreateItem(string UserId, string categoryslug, string title, string description, string provinceId, string districtId, string whenTypeId, string whenDateId, string whenTimeId, IEnumerable<CategoryQuestions> questions, decimal? price, IEnumerable<IFormFile> galleryImages);

        TransactionResult EditItem(string UserId, Guid itemId, string categoryslug, string title, string description, string provinceId, string districtId, string whenTypeId, string whenDateId, string whenTimeId, IEnumerable<CategoryQuestions> questions, decimal? price, IEnumerable<IFormFile> galleryImages);

        TransactionResult RemoveItemGallery(string userID, Guid itemId, string url);

        IEnumerable<Item> SearchItems(SearchViewModel model);

        IEnumerable<Item> SearchItems(
            string keyword,
            string zone,
            Guid? categoryId = null,
            int? provinceId = null,
            int? districtId = null,
            SearchSortTypes sortOrder = SearchSortTypes.Newest,
            IEnumerable<string> categories = null,
            int page = 0);

        IEnumerable<Item> GetUserItems(string UserId, ProjectFilterTypes FilterType);

        IEnumerable<Offer> GetUserOffers(string UserId, OfferFilterTypes FilterType);

        IEnumerable<TokenItem> GetUserTokens(string UserId);

        IEnumerable<string> GetProvinceDistrict();

        IEnumerable<Province> GetProvinces();

        IEnumerable<SelectListItem> GetProvincesSelectList();

        IEnumerable<SelectListItem> GetDistrictsSelectList(int? provinceId);

        IEnumerable<SelectListItem> GetUserCategories(bool addEmpty = false);

        IEnumerable<CategoryQuestions> GetCategoryQuestions(string categoryslug);

        IEnumerable<SelectListItem> GetWhenTypes();

        IEnumerable<SelectListItem> GetWhenDates();

        IEnumerable<SelectListItem> GetWhenTimes();

        IEnumerable<SelectListItem> GetSearchSortTypes();

        IEnumerable<SelectListItem> GetProjectFilterTypes();

        IEnumerable<SelectListItem> GetOfferFilterTypes();

        TransactionResult UpdateItemStatus(Guid id, StatusTypes status);
    }
}