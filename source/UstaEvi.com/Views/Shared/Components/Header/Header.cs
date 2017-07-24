using ExpertFinder.Common.ViewModels.Components;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.Header
{
    public class Header : ViewComponent
    {
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;
        private readonly ICategoryServices _categoryServices;

        public Header(
            UserManager<User> userManager,
            SignInManager<User> signInManager,
            ICategoryServices categoryServices)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _categoryServices = categoryServices;
        }

        //IEnumerable<Page> pages
        async public Task<IViewComponentResult> InvokeAsync()
        {
            return View(new HeaderViewModel()
            {
                Categories = _categoryServices.GetPopularCategories()
            });
        }
    }
}