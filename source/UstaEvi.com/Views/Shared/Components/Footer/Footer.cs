using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace UstaEvi.com.Views.Shared.Components.Footer
{
    public class Footer : ViewComponent
    {
        public Footer()
        {
        }

        async public Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}