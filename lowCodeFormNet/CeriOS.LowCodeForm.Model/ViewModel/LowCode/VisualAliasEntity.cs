using CeriOS.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    [SugarTable("BASE_VISUAL_ALIAS")]
    public class VisualAliasEntity:PublicProperty
    {
        /// <summary>
        /// 获取或设置 编号.
        /// </summary>
        [SugarColumn(ColumnName = "F_ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 功能表单id.
        /// </summary>
        [SugarColumn(ColumnName = "F_VISUAL_ID")]
        public string VisualId { get; set; }

        /// <summary>
        /// 表或字段别名.
        /// </summary>
        [SugarColumn(ColumnName = "F_ALIAS_NAME")]
        public string AliasName { get; set; }

        /// <summary>
        /// 表名称.
        /// </summary>
        [SugarColumn(ColumnName = "F_TABLE_NAME")]
        public string TableName { get; set; }

        /// <summary>
        /// 字段名称.
        /// </summary>
        [SugarColumn(ColumnName = "F_FIELD_NAME")]
        public string FieldName { get; set; }
    }
}
