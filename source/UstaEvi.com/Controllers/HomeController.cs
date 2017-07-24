using ExpertFinder.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace UstaEvi.com.Controllers
{
    public class HomeController : Controller
    {
        private readonly ICategoryServices _categoryServices;

        public HomeController(ICategoryServices categoryServices)
        {
            _categoryServices = categoryServices;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Index(string keyword)
        {
            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var possibleCategories = keyword.Split(' ');

                foreach (var item in possibleCategories)
                {
                    var category = _categoryServices.GetCategories().FirstOrDefault(i => i.Name.ToLowerInvariant().Split(' ').Contains(item.ToLowerInvariant()));

                    if (category != null)
                    {
                        return RedirectToRoute("createItem", new { categoryslug = category.SlugUrl, title = keyword });
                    }
                }
            }

            return RedirectToRoute("createItem", new { title = keyword });
        }
    }
}