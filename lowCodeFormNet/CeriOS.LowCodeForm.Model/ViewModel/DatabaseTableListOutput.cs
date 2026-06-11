using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    /// <summary>
    /// 数据库表列表输出.
    /// </summary>
    public class DatabaseTableListOutput
    {
        /// <summary>
        /// 说明.
        /// </summary>
        public string? description { get; set; }

        /// <summary>
        /// 表记录数.
        /// </summary>
        public int sum { get; set; }

        /// <summary>
        /// 表名.
        /// </summary>
        public string? table { get; set; }

        /// <summary>
        /// 表说明.
        /// </summary>
        public string? tableName { get; set; }

        /// <summary>
        /// 0-表 1-视图.
        /// </summary>
        public int? type { get; set; }
    }

}
