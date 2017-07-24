using Common.Entities;
using ExpertFinder.Common.AdminViewModel;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpertFinder.Application.Interfaces
{
    public interface IAdminEngine
    {
        TransactionResult ChangeContent(ContentsFormViewModel model);

        IEnumerable<User> GetAllEmploye();

        Task<TransactionResult> AddEmploye(string userId, EmployeFormViewModel model);

        Task<TransactionResult> UpdateEmploye(EmployeFormViewModel model);

        IEnumerable<Campain> GetCampaign(bool? status);

        Campain GetCampaignDetail(int id);

        TransactionResult AddCampaign(CampaignFormViewModel model, IFormFile headerImag);

        TransactionResult UpdateCampaign(CampaignFormViewModel model, IFormFile headerImag);

        IEnumerable<Category> GetCategories(bool? isRoot);

        Category GetCategory(Guid id);

        TransactionResult AddCategory(CategoryFormViewModel model, IFormFile headerImage);

        TransactionResult UpdateCategory(CategoryFormViewModel model, IFormFile headerImage);

        IEnumerable<Category> GetSubCategories(Guid parantId);

        IEnumerable<CategoryQuestions> GetCategoryQuestions(Guid categoryId);

        TransactionResult AddCategoryQuestion(CategoryQuestions categoryQuestions);

        TransactionResult RemoveAllCategoryQuestion(Guid categoryId);

        IEnumerable<Province> GetProvince();

        IEnumerable<District> GetDistrict(int province);

        IEnumerable<User> SearchEmploye(
            string name,
            string email,
            int? provinceId = null,
            int? districtId = null,
            int page = 0);

        User GetMemberDetail(string id);

        TransactionResult AddToken(Guid id, int tokenCount);

        TransactionResult UpdateEmployeStatus(Guid id, string LockoutEnabled);

        IEnumerable<Offer> GetMemberOffers(string id);

        List<Category> GetMemberCategoryRelationList(string id);

        IEnumerable<Offer> GetItemOffers(string offerId);

        Item GetItemByOfferId(string offerId);

        IEnumerable<Offer> GetItemOffersByItemId(string itemId);

        Item GetItemById(string itemId);

        IEnumerable<User> GetUserByName(string username);

        IEnumerable<User> GetUserByEmail(string useremail);

        IEnumerable<Category> GetRootCategoriesByName(string categoryName);

        IEnumerable<Category> GetSubCategoriesByName(Guid parantId, string subCategoryName);

        IEnumerable<Offer> SearchOffer(
         string user,
         string userEmail,
         string category,
         string subCategory,
         int page = 0);

        IEnumerable<Item> SearchItem(
          string user,
          string userEmail,
          string category,
          string subCategory,
          string status,
          int page = 0);

        int GetMemberTokenCount(string userId);

        IEnumerable<Item> GetMemberItems(string userId);

        TransactionResult UpdateItemStatus(Guid id);

        IEnumerable<User> GetUserForSMS(int? province, int? district, string category, string subCategory);
    }
}