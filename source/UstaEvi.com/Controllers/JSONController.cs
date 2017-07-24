using Common.Web.Data;
using Common.Web.IO;
using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.Enums;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;

// For more information on enabling Web API for empty projects, visit http://go.microsoft.com/fwlink/?LinkID=397860

namespace UstaEvi.com.Controllers
{
    [Route("api/[controller]")]
    public class JSONController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly IProjectEngine _projectEngine;
        private readonly IUserEngine _userEngine;
        private readonly ICachingEx _cachingEx;
        private readonly IHostingEnvironment _environment;

        public JSONController(
            UserManager<User> userManager,
            IProjectEngine projectEngine,
            IUserEngine userEngine,
            ICachingEx cachingEx,
            IHostingEnvironment environment
            )
        {
            _userManager = userManager;
            _projectEngine = projectEngine;
            _userEngine = userEngine;
            _cachingEx = cachingEx;
            _environment = environment;
        }

        [HttpPost("UploadImage/{type}/{itemid}/{filename}")]
        public JsonResult UploadImage(IFormFile file, PageContentTypes type, string itemid, string filename)
        {
            filename = Path.GetFileNameWithoutExtension(filename);
            var upload = new FormUpload(_environment.WebRootPath); if (file != null)
            {
                switch (type)
                {
                    case PageContentTypes.Campaign:
                        upload.SaveFile(file, filename, "campaign");
                        break;

                    case PageContentTypes.Category:
                        upload.SaveFile(file, filename, "categoryheader");
                        break;
                }
            }

            return Json("OK");
        }

        [HttpGet("ClearCache/{key}")]
        public JsonResult ClearCache(string key)
        {
            if (_cachingEx.Exists(key))
            {
                _cachingEx.Remove(key);
            }

            return Json("OK");
        }

        [HttpGet("FillDistrict/{districtId?}")]
        public JsonResult FillDistrict(int? districtId)
        {
            return Json(_projectEngine.GetDistrictsSelectList(districtId));
        }

        [HttpGet("GetProvinceDistrict")]
        public JsonResult GetProvinceDistrict()
        {
            return Json(_projectEngine.GetProvinceDistrict());
        }

        [HttpGet("RemoveGallery/{id}")]
        public JsonResult RemoveGallery(long id)
        {
            return Json(_userEngine.RemoveGalleryItem(_userManager.GetUserId(User), id));
        }

        [HttpGet("RemoveItemGallery/{itemId}/{url}")]
        public JsonResult RemoveItemGallery(Guid itemId, string url)
        {
            return Json(_projectEngine.RemoveItemGallery(_userManager.GetUserId(User), itemId, url));
        }
    }
}