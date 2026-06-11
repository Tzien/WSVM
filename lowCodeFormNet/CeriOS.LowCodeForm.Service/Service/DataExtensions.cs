using Mapster;
using MapsterMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public static class DataExtensions
    {
        public static IServiceCollection AddObjectMapper(this IServiceCollection services, params Assembly[] assemblies)
        {
            var config = TypeAdapterConfig.GlobalSettings;

            if (assemblies != null && assemblies.Length > 0) config.Scan(assemblies);

            config.Default
                  .NameMatchingStrategy(NameMatchingStrategy.Flexible)
                  .PreserveReference(true);

            // 配置默认全局映射（忽略大小写敏感）
            config.Default
                  .NameMatchingStrategy(NameMatchingStrategy.IgnoreCase)
                  .PreserveReference(true);

            // 配置支持依赖注入
            services.AddSingleton(config);
            services.AddScoped<IMapper, ServiceMapper>();

            return services;
        }
    }
}
