using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.Content
{
    public class Content : ViewComponent
    {
        private readonly IContentEngine _contentEngine;

        public Content(IContentEngine contentEngine)
        {
            _contentEngine = contentEngine;
        }

        async public Task<IViewComponentResult> InvokeAsync(string key, ExpertFinder.Models.Content content)
        {
            if (content == null)
                content = _contentEngine.Contents[key];

            return View(new ContentViewModel
            {
                Title = content.Title,
                Content = content.Value
            });
        }
    }
}