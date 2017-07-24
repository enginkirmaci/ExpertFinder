using Autofac;
using Autofac.Extensions.DependencyInjection;
using Common.Web.ActionFilters;
using Common.Web.Middlewares;
using ExpertFinder.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using UstaEvi.com.Infrastructure;
using UstaEvi.com.Resources;
using ILogger = Microsoft.Extensions.Logging.ILogger;

namespace UstaEvi.com
{
    public class Startup
    {
        public IConfiguration Configuration { get; }
        private readonly ILogger Logger;

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: true);

            if (env.IsDevelopment())
            {
                builder.AddUserSecrets<Startup>();
            }

            builder.AddEnvironmentVariables();
            Configuration = builder.Build();

            // Setup logger
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.MSSqlServer(Configuration["Data:DefaultConnection:ConnectionString"], "Logs", LogEventLevel.Verbose, 50, null, null, false,
                new Serilog.Sinks.MSSqlServer.ColumnOptions()
                {
                    AdditionalDataColumns = new DataColumn[]
                    {
                        new DataColumn { DataType = typeof(string), ColumnName = "Code" },
                        new DataColumn { DataType = typeof(string), ColumnName = "CreatedBy" },
                    }
                })
                .CreateLogger();

            Logger = new LoggerFactory()
                .AddSerilog()
                .CreateLogger<Program>();
        }

        public IServiceProvider ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<teklifcepteDBContext>(options =>
                     options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"], b => b.MigrationsAssembly("ExpertFinder.Models")));

            services.AddIdentity<User, IdentityRole>(i =>
            {
                i.Password.RequireDigit = false;
                i.Password.RequiredLength = 6;
                i.Password.RequireLowercase = false;
                i.Password.RequireNonAlphanumeric = false;
                i.Password.RequireUppercase = false;

                //i.SignIn.RequireConfirmedPhoneNumber = true;

                //i.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                //i.Lockout.MaxFailedAccessAttempts = 10;
            })
            .AddEntityFrameworkStores<teklifcepteDBContext>()
            .AddDefaultTokenProviders()
            .AddErrorDescriber<TrIdentityErrorDescriber>();

            services.Configure<IdentityOptions>(options =>
            {
                options.Cookies.ApplicationCookie.LoginPath = new PathString("/giris");
                options.Cookies.ApplicationCookie.AccessDeniedPath = new PathString("/giris");
                options.Cookies.ApplicationCookie.LogoutPath = new PathString("/cikis");
            });

            services.AddMemoryCache();
            services.AddSession();
            services.AddMvc(options =>
            {
                options.Filters.Add(typeof(ValidateAjaxAttribute));
            });

            services.Configure<RouteOptions>(options =>
            {
                options.LowercaseUrls = true;
                options.AppendTrailingSlash = true;
            });

            services.AddSingleton(_ => Configuration);

            services.AddSingleton(_ => Logger);

            // Adds Autofac
            var containerBuilder = new ContainerBuilder();
            containerBuilder.RegisterModule(new AutofacModule(Configuration));
            containerBuilder.Populate(services);
            var container = containerBuilder.Build();
            return container.Resolve<IServiceProvider>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                SupportedCultures = new List<CultureInfo> { new CultureInfo("tr-TR") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("tr-TR") },
                DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(new CultureInfo("tr-TR"))
            });

            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseExceptionHandler("/Error");
            }

            app.UseStaticFiles();
            app.UseIdentity();
            app.UseMiddleware<NonWwwRedirectMiddleware>();
            app.UseMiddleware<ErrorLoggingMiddleware>();

            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AutomaticAuthenticate = true,
                AutomaticChallenge = true
                //LoginPath = new PathString("/giris"),
                //AccessDeniedPath = new PathString("/giris"),
                //LogoutPath = new PathString("/cikis")
            });

            //app.UseMicrosoftAccountAuthentication(options =>
            //{
            //    options.Scope.Add("wl.basic");
            //    options.Scope.Add("wl.emails");

            //    options.Description.Items.Add("icon", "fa-windows");
            //    options.Description.Items.Add("class", "sb-windows");

            //    options.ClientId = Configuration["Authentication:Microsoft:AppId"];
            //    options.ClientSecret = Configuration["Authentication:Microsoft:AppSecret"];

            //    options.CallbackPath = new PathString("/account/externallogincallback");
            //});

            //app.UseGoogleAuthentication(options =>
            //{
            //    options.Description.Items.Add("icon", "fa-google");
            //    options.Description.Items.Add("class", "sb-googleplus");

            //    options.ClientId = Configuration["Authentication:Google:ClientId"];
            //    options.ClientSecret = Configuration["Authentication:Google:ClientSecret"];

            //    options.CallbackPath = new PathString("/account/externallogincallback/");
            //});

            //app.UseTwitterAuthentication(options =>
            //{
            //    options.Description.Items.Add("icon", "fa-twitter");
            //    options.Description.Items.Add("class", "sb-twitter");

            //    options.ConsumerKey = Configuration["Authentication:Twitter:ConsumerKey"];
            //    options.ConsumerSecret = Configuration["Authentication:Twitter:ConsumerSecret"];
            //});

            var options = new FacebookOptions
            {
                AppId = Configuration["Authentication:Facebook:AppId"],
                AppSecret = Configuration["Authentication:Facebook:AppSecret"],

                UserInformationEndpoint = "https://graph.facebook.com/me?fields=id,name,email",
            };

            options.Scope.Add("email");
            options.Scope.Add("public_profile");

            options.Description.Items.Add("icon", "fa-facebook");
            options.Description.Items.Add("class", "sb-facebook");

            app.UseFacebookAuthentication(options);

            app.UseSession();

            var initializer = new teklifcepteDBInitializer();
            initializer.InitializeDatabase(app.ApplicationServices);

            app.UseMvc(RouteConfig.RegisterRoutes);
        }
    }
}