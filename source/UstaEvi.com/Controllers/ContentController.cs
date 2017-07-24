using Common.Utilities.Converters;
using ExpertFinder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace UstaEvi.com.Controllers
{
    public class ContentController : BaseController
    {
        public ContentController(IContentEngine contentEngine) : base(contentEngine)
        {
        }

        public IActionResult Index()
        {
            var key = UrlConverter.GetFirstSegment(Request.Path);
            SetPageContent(key);

            return View();
        }
    }
}