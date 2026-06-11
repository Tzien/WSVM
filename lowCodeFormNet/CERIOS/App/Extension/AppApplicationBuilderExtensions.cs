using Microsoft.AspNetCore.Builder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.App.Extension
{
    /// <summary>
    /// 应用中间件拓展类（由框架内部调用）
    /// </summary>
    public static class AppApplicationBuilderExtensions
    {
        /// <summary>
        /// 添加应用中间件
        /// </summary>
        /// <param name="app">应用构建器</param>
        /// <param name="configure">应用配置</param>
        /// <returns><see cref="IApplicationBuilder"/></returns>
        internal static IApplicationBuilder UseApp(this IApplicationBuilder app, Action<IApplicationBuilder> configure = null)
        {
            // 调用自定义服务
            configure?.Invoke(app);
            return app;
        }
    }
}
