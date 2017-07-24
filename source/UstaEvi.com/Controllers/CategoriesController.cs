using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.ViewModels;
using ExpertFinder.Domain.Interfaces;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace UstaEvi.com.Controllers
{
    public class CategoriesController : BaseController
    {
        private readonly IProjectEngine _projectEngine;
        private readonly ICategoryServices _categoryServices;

        public CategoriesController(
            IProjectEngine projectEngine,
            IContentEngine contentEngine,
            ICategoryServices categoryServices
            ) : base(contentEngine)
        {
            _projectEngine = projectEngine;
            _categoryServices = categoryServices;
        }

        [Route("kategoriler/{model?}", Name = "categories")]
        [HttpGet]
        public IActionResult Index()
        {
            return View(new CategoriesViewModel()
            {
                Categories = _categoryServices.GetCategories().Where(i => !i.ParentId.HasValue)
            });
        }

        [Route("kategori/{category}", Name = "category")]
        [HttpGet]
        public IActionResult Index(string category)
        {
            var pageNumber = 0;
            var obj = _categoryServices.GetCategory(category);

            if (Request.HasFormContentType && Request.Form.ContainsKey("paging"))
                pageNumber = int.Parse(Request.Form["paging"]);

            var items = _projectEngine.SearchItems(null, null, categoryId: obj.Id, page: pageNumber);

            return View(new CategoriesViewModel()
            {
                CurrentCategory = obj,
                ImageUrl = obj.ImageUrl,
                Categories = new List<Category>() { obj },
                Items = items,
                PageNumber = pageNumber
            });
        }

        [Route("kategori/{category}/{subcategory}", Name = "subcategory")]
        public IActionResult Index(string category, string subcategory)
        {
            var pageNumber = 0;
            var obj = _categoryServices.GetCategory(subcategory);

            if (Request.HasFormContentType && Request.Form.ContainsKey("paging"))
                pageNumber = int.Parse(Request.Form["paging"]);

            var items = _projectEngine.SearchItems(null, null, categoryId: obj.Id, page: pageNumber);

            return View(new CategoriesViewModel()
            {
                CurrentCategory = obj,
                ImageUrl = !string.IsNullOrWhiteSpace(obj.ImageUrl) ? obj.ImageUrl : obj.Parent.ImageUrl,
                Categories = new List<Category>() { obj },
                Items = items,
                PageNumber = pageNumber
            });
        }
    }
}