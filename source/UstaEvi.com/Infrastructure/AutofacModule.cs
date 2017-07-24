using Autofac;
using Common.Web.Data;
using Common.Web.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Reflection;

namespace UstaEvi.com.Infrastructure
{
    public class AutofacModule : Autofac.Module
    {
        public IConfiguration _configuration { get; }

        public AutofacModule(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        protected override void Load(ContainerBuilder builder)
        {
            builder.Register(i => new SenGridEmailSender()
                .Configure(
                    _configuration["Email:SendGrid:Key"],
                    _configuration["Email:SendGrid:FromEmail"],
                    _configuration["Email:SendGrid:FromName"],
                    _configuration["Email:SendGrid:DefaultTemplate"]
                ))
                .As<IEmailSender>()
                .InstancePerLifetimeScope();

            builder.Register(i => new CachingEx(i.Resolve<IMemoryCache>())).As<ICachingEx>().SingleInstance();
            builder.Register(i => new SessionEx(i.Resolve<IHttpContextAccessor>())).As<ISessionEx>().SingleInstance();

            var assemblies = new List<Assembly>();
            var referenced = Assembly.GetEntryAssembly().GetReferencedAssemblies();

            foreach (var assembly in referenced)
                if (assembly.Name.Equals("ExpertFinder.Application"))
                {
                    assemblies.Add(Assembly.Load(assembly));
                }

            builder.RegisterAssemblyTypes(assemblies.ToArray()).AsImplementedInterfaces();
        }
    }
}