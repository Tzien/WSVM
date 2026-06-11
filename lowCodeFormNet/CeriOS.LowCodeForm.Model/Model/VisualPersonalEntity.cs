using CeriOS.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.Model
{
    /// <summary>
    /// 列表个性视图.
    /// </summary>
    [SugarTable("BASE_VISUAL_PERSONAL")]
    public class VisualPersonalEntity : PublicProperty
    {
        /// <summary>
        /// id.
        /// </summary>
        [SugarColumn(ColumnName = "F_ID",IsPrimaryKey = true)]
        public string F_ID { get; set; }

        /// <summary>
        /// 菜单id.
        /// </summary>
        [SugarColumn(ColumnName = "F_MENU_ID")]
        public string MenuId { get; set; }

        /// <summary>
        /// 视图名称.
        /// </summary>
        [SugarColumn(ColumnName = "F_FULL_NAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 类型：0-系统，1-其他.
        /// </summary>
        [SugarColumn(ColumnName = "F_TYPE")]
        public int? Type { get; set; }

        /// <summary>
        /// 状态：0-其他，1-默认.
        /// </summary>
        [SugarColumn(ColumnName = "F_STATUS")]
        public int? Status { get; set; }

        /// <summary>
        /// 查询字段.
        /// </summary>
        [SugarColumn(ColumnName = "F_SEARCH_LIST")]
        public string SearchList { get; set; }

        /// <summary>
        /// 列表字段.
        /// </summary>
        [SugarColumn(ColumnName = "F_COLUMN_LIST")]
        public string ColumnList { get; set; }
    }
}
