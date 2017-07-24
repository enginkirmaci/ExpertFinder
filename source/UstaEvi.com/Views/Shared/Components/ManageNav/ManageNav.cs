using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.ManageNav
{
    public class ManageNav : ViewComponent
    {
        public ManageNav()
        {
        }

        async public Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}