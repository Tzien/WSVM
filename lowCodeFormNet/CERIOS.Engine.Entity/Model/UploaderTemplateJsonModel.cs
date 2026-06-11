using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.Engine.Entity.Model
{
    /// <summary>
    /// 数据导入模板模型
    /// </summary>
    public class UploaderTemplateJsonModel
    {
        /// <summary>
        /// 导入类型.
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 导入列名 集合.
        /// </summary>
        public List<string> selectKey { get; set; }
    }
}
