using CERIOS.App.Attributes;
using CERIOS.App.Options;
using CERIOS.App.Startups;
using CERIOS.ConfigurableOptions.Extensions;
using CERIOS.DependencyInjection.Extensions;
using CERIOS.ObjectMapper.Extensions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.App.Extension
{
    /// <summary>
    /// 应用服务集合拓展类（由框架内部调用）
    /// </summary>
    public static class AppServiceCollectionExtensions
    {
        /// <summary>
        /// 添加应用配置
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <param name="configure">服务配置</param>
        /// <returns>服务集合</returns>
        internal static IServiceCollection AddApp(this IServiceCollection services, Action<IServiceCollection> configure = null)
        {
            // 注册全局配置选项
            services.AddConfigurableOptions<AppSettingsOptions>();

            // 注册内存和分布式内存
            services.AddMemoryCache();
            services.AddDistributedMemoryCache();

            // 注册全局依赖注入
            services.AddDependencyInjection();

            // 注册全局 Startup 扫描
            services.AddStartups();

            // 添加对象映射
            services.AddObjectMapper();

            // 默认内置 GBK，Windows-1252, Shift-JIS, GB2312 编码支持
            Encoding.RegisterProvider(CodePagesEncodingProvider.Instance);

            // 自定义服务
            configure?.Invoke(services);

            return services;
        }

        /// <summary>
        /// 添加 Startup 自动扫描
        /// </summary>
        /// <param name="services">服务集合</param>
        /// <returns>服务集合</returns>
        internal static IServiceCollection AddStartups(this IServiceCollection services)
        {
            // 扫描所有继承 AppStartup 的类
            var startups = App.EffectiveTypes
                .Where(u => typeof(AppStartup).IsAssignableFrom(u) && u.IsClass && !u.IsAbstract && !u.IsGenericType)
                .OrderByDescending(u => GetStartupOrder(u));

            // 注册自定义 startup
            foreach (var type in startups)
            {
                var startup = Activator.CreateInstance(type) as AppStartup;
                App.AppStartups.Add(startup);

                // 获取所有符合依赖注入格式的方法，如返回值void，且第一个参数是 IServiceCollection 类型
                var serviceMethods = type.GetMethods(BindingFlags.Public | BindingFlags.Instance)
                    .Where(u => u.ReturnType == typeof(void)
                        && u.GetParameters().Length > 0
                        && u.GetParameters().First().ParameterType == typeof(IServiceCollection));

                if (!serviceMethods.Any()) continue;

                // 自动安装属性调用
                foreach (var method in serviceMethods)
                {
                    method.Invoke(startup, new[] { services });
                }
            }

            return services;
        }

        /// <summary>
        /// 获取 Startup 排序
        /// </summary>
        /// <param name="type">排序类型</param>
        /// <returns>int</returns>
        private static int GetStartupOrder(Type type)
        {
            return !type.IsDefined(typeof(AppStartupAttribute), true) ? 0 : type.GetCustomAttribute<AppStartupAttribute>(true).Order;
        }

        /// <summary>
        /// 自动添加主机服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddAppHostedService(this IServiceCollection services)
        {
            // 获取所有 BackgroundService 类型，排除泛型主机
            var backgroundServiceTypes = App.EffectiveTypes.Where(u => typeof(IHostedService).IsAssignableFrom(u) && u.Name != "GenericWebHostService");
            var addHostServiceMethod = typeof(ServiceCollectionHostedServiceExtensions).GetMethods(BindingFlags.Static | BindingFlags.Public)
                                .Where(u => u.Name.Equals("AddHostedService") && u.IsGenericMethod && u.GetParameters().Length == 1)
                                .FirstOrDefault();

            foreach (var type in backgroundServiceTypes)
            {
                addHostServiceMethod.MakeGenericMethod(type).Invoke(null, new object[] { services });
            }

            return services;
        }
    }
}
