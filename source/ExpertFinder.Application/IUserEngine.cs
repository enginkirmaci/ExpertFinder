using Common.Entities;
using ExpertFinder.Common.ViewModels.Manage;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ExpertFinder.Application.Interfaces
{
    public interface IUserEngine
    {
        void ClearUserSession(string UserId);

        Task<User> CurrentUser(string UserId);

        void ClearUserToken(string UserId);

        int CurrentUserToken(string UserId, bool disableCache = false);

        Task<User> GetUser(string slugUrl);

        IEnumerable<UserExperienceImages> GetGallery(string UserID);

        TransactionResult RemoveGalleryItem(string userID, long itemId);

        void AfterRegistration(string userID, string notificationDescription = null, IEnumerable<string> SelectedUserCategories = null);

        TransactionResult SaveUser(ProfileViewModel model, IFormFile headerImage, IFormFile avatarImage, IEnumerable<IFormFile> galleryImages);

        TransactionResult ChangeSmsSetting(string userID, bool smsNotAllowed);
    }
}