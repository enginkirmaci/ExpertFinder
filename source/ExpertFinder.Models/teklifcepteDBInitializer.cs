using Common.Database;
using System;

namespace ExpertFinder.Models
{
    public class teklifcepteDBInitializer : ApplicationDbInitializer
    {
        public void InitializeDatabase(IServiceProvider serviceProvider)
        {
            base.InitializeDatabaseAsync<teklifcepteDBContext, User>(serviceProvider).Wait();

            //CreateAdminUser(serviceProvider).Wait();
        }

        //private async Task CreateAdminUser(IServiceProvider serviceProvider)
        //{
        //    var appEnv = serviceProvider.GetService<IApplicationEnvironment>();
        //    var configuration = serviceProvider.GetService<IConfiguration>();

        //    var db = serviceProvider.GetService<teklifcepteDBContext>();
        //    var website = db.Website.FirstOrDefault(i => i.Id == new Guid(configuration["Website:Guid"]));
        //    if (website == null)
        //    {
        //        website = new Website
        //        {
        //            Id = new Guid(configuration["Website:Guid"])
        //        };

        //        db.Website.Add(website);
        //        db.SaveChanges();

        //        var userManager = serviceProvider.GetService<UserManager<User>>();

        //        var user = await userManager.FindByNameAsync(configuration["Admin:Username"]);

        //        if (user == null)
        //        {
        //            user = new User
        //            {
        //                UserName = configuration["Admin:Username"],
        //                WebsiteId = website.Id
        //            };

        //            var result = await userManager.CreateAsync(user, configuration["Admin:Password"]);
        //            if (result.Succeeded)
        //                await userManager.AddClaimAsync(user, new Claim("Admin", "Allowed"));
        //        }
        //    }
        //}
    }
}