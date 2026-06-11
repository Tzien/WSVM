using CERIOS.ConfigurableOptions.Options;
using CERIOS.DependencyInjection.Internal;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.DependencyInjection.Options
{
    /// <summary>
    /// 依赖注入配置选项
    /// </summary>
    public sealed class DependencyInjectionSettingsOptions : IConfigurableOptions<DependencyInjectionSettingsOptions>
    {
        /// <summary>
        /// 外部注册定义
        /// </summary>
        public ExternalService[] Definitions { get; set; }

        /// <summary>
        /// 后期配置
        /// </summary>
        /// <param name="options"></param>
        /// <param name="configuration"></param>
        public void PostConfigure(DependencyInjectionSettingsOptions options, IConfiguration configuration)
        {
            options.Definitions ??= Array.Empty<ExternalService>();
        }
    }
}
