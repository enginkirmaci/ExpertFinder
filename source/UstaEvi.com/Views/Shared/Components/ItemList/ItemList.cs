using ExpertFinder.Common.ViewModels.Components;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.ItemList
{
    public class ItemList : ViewComponent
    {
        public ItemList()
        {
        }

        async public Task<IViewComponentResult> InvokeAsync(int pageNumber, IEnumerable<Item> items)
        {
            return View(new ItemListViewModel()
            {
                Result = items,
                PageNumber = pageNumber
            });
        }
    }
}