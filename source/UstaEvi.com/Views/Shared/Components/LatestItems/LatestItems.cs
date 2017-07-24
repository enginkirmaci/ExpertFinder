using ExpertFinder.Application.Interfaces;
using ExpertFinder.Common.ViewModels.Components;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.LatestItems
{
    public class LatestItems : ViewComponent
    {
        private readonly IProjectEngine _projectEngine;

        public LatestItems(IProjectEngine projectEngine)
        {
            _projectEngine = projectEngine;
        }

        async public Task<IViewComponentResult> InvokeAsync()
        {
            //TODO add caching here
            return View(new ItemListViewModel()
            {
                Result = _projectEngine.SearchItems(string.Empty, string.Empty).Take(5)
            });
        }
    }
}