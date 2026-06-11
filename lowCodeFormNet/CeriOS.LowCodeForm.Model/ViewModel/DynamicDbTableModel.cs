using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    public class DynamicDbTableModel
    {
        /// <summary>
        /// 表名.
        /// </summary>
        public string? F_TABLE { get; set; }

        /// <summary>
        /// 表说明.
        /// </summary>
        public string? F_TABLENAME { get; set; }

        /// <summary>
        /// 大小.
        /// </summary>
        public string? F_SIZE { get; set; }

        /// <summary>
        /// 总数.
        /// </summary>
        public string? F_SUM { get; set; }

        /// <summary>
        /// 主键.
        /// </summary>
        public string? F_PRIMARYKEY { get; set; }
    }
}
