using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.ConfigurableOptions.Attributes
{
    /// <summary>
    /// 重新映射属性配置
    /// </summary>
    public sealed class MapSettingsAttribute : Attribute
    {

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="path">appsetting.json 对应键</param>
        public MapSettingsAttribute(string path)
        {
            Path = path;
        }

        /// <summary>
        /// 对应配置文件中的路径
        /// </summary>
        public string Path { get; set; }
    }
}
