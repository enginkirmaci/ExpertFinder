using Common.Utilities.Converters;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

// For more information on enabling MVC for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace UstaEvi.com.Controllers
{
    public class WhereWeController : BaseController
    {
        private readonly IProjectEngine _projectEngine;

        // GET: /<controller>/
        public WhereWeController(
            IProjectEngine projectEngine,
            IContentEngine contentEngine
            ) : base(contentEngine)
        {
            _projectEngine = projectEngine;
        }

        [Route("nerelerdeyiz", Name = "wherewe")]
        [HttpGet]
        public IActionResult Index()
        {
            return View(new WhereWeViewModel()
            {
                Provinces = _projectEngine.GetProvinces()
            });
        }

        [Route("nerelerdeyiz/{province}", Name = "whereweprovince")]
        [HttpGet]
        public IActionResult Index(string province)
        {
            var obj = _projectEngine.GetProvinces().FirstOrDefault(i => UrlConverter.ToUrlSlug(i.Name) == province);

            return View(new WhereWeViewModel()
            {
                CurrentProvince = obj
            });
        }

        [Route("nerelerdeyiz/{province}/{district}", Name = "wherewedistrict")]
        public IActionResult Index(string province, string district)
        {
            var pageNumber = 0;
            var obj = _projectEngine.GetProvinces().FirstOrDefault(i => UrlConverter.ToUrlSlug(i.Name) == province);
            var districtObj = obj.District.FirstOrDefault(i => UrlConverter.ToUrlSlug(i.Name) == district);

            if (Request.HasFormContentType && Request.Form.ContainsKey("paging"))
                pageNumber = int.Parse(Request.Form["paging"]);

            var items = _projectEngine.SearchItems(null, null, provinceId: obj.Id, districtId: districtObj.Id, page: pageNumber);

            return View(new WhereWeViewModel()
            {
                CurrentProvince = obj,
                CurrentDistrict = districtObj,
                Items = items,
                PageNumber = pageNumber
            });
        }
    }
}