using Common.Entities;
using Common.Entities.Enums;
using ExpertFinder.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace UstaEvi.com.Controllers
{
    public class BaseController : Controller
    {
        private readonly IContentEngine _contentEngine;

        public BaseController(IContentEngine contentEngine)
        {
            _contentEngine = contentEngine;
        }

        public void SetPageContent(string key)
        {
            if (_contentEngine.Contents.ContainsKey(key))
            {
                var content = _contentEngine.Contents[key];

                if (!string.IsNullOrWhiteSpace(content.Title))
                {
                    ViewBag.Title = content.Title;
                }
                ViewBag.Content = content;
            }
        }

        public IActionResult SetTransactionResult(string message, string returnUrl, TransactionType type)
        {
            ViewBag.Transaction = new TransactionResult()
            {
                Message = message,
                ReturnUrl = returnUrl,
                Type = type
            };

            return View();
        }
    }
}