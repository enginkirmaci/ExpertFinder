using ExpertFinder.Common.ViewModels.Components;
using ExpertFinder.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.PopularCategories
{
    public class Categories : ViewComponent
    {
        private readonly ICategoryServices _categoryServices;

        public Categories(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        //IEnumerable<Page> pages
        async public Task<IViewComponentResult> InvokeAsync()
        {
            return View(new CategoriesComponentViewModel()
            {
                Categories = _categoryServices.GetCategories()
            });
        }
    }
}