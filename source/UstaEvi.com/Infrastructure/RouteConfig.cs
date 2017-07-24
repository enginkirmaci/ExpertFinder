using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;

namespace UstaEvi.com.Infrastructure
{
    public class RouteConfig
    {
        public static void RegisterRoutes(IRouteBuilder routes)
        {
            routes.MapRoute(
                name: "default",
                template: "{controller=Home}/{action=Index}/{id?}");

            routes.MapRoute(
                name: "home",
                template: "",
                defaults: new
                {
                    controller = "Home",
                    action = "Index"
                }
            );

            routes.MapRoute(
                name: "error",
                template: "hata/{id?}",
                defaults: new
                {
                    controller = "Error",
                    action = "Index"
                });

            routes.MapRoute(
                name: "contentPages",
                template: "{*page}",
                defaults: new
                {
                    controller = "Content",
                    action = "Index"
                });
        }
    }
}