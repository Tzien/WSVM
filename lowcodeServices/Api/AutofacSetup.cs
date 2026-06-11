using Autofac.Extensions.DependencyInjection;
using Autofac;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Common.Helper;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Hosting;
using System.Reflection;

namespace Api
{
    public static class AutofacSetup
    {
        public static IHostBuilder AddAutofacSetup2(this WebApplicationBuilder builder)
        {
            return builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureContainer<ContainerBuilder>(b =>
                {
                    var assemblysService = Assembly.Load(AppConfigurtaionServices.GetValue("ServiceName"));
                    b.RegisterAssemblyTypes(assemblysService)
                               .AsImplementedInterfaces()
                               .InstancePerLifetimeScope();
                });

        }
    }
}
