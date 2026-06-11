using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.Engine.Entity.Model
{
    /// <summary>
    /// 显示列模型.
    /// </summary>
    public class IndexGridFieldModel : IndexEachConfigBase
    {
        /// <summary>
        /// id.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 对齐.
        /// </summary>
        public string align { get; set; }

        /// <summary>
        /// 固定.
        /// </summary>
        public string @fixed { get; set; }

        /// <summary>
        /// 宽度.
        /// </summary>
        public int? width { get; set; }

        /// <summary>
        /// 是否排序.
        /// </summary>
        public bool sortable { get; set; }

        /// <summary>
        /// 当前位置.
        /// </summary>
        public int currentIndex { get; set; }
    }
}
