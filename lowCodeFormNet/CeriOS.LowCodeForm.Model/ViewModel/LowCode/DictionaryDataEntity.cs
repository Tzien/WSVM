using CeriOS.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    public  class DictionaryDataEntity:PublicProperty
    {
        /// <summary>
        /// 获取或设置 编号.
        /// </summary>
        [SugarColumn(ColumnName = "F_ID", ColumnDescription = "主键", IsPrimaryKey = true)]
        public string Id { get; set; }
        /// <summary>
        /// 上级.
        /// </summary>
        [SugarColumn(ColumnName = "F_PARENT_ID")]
        public string ParentId { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        [SugarColumn(ColumnName = "F_FULL_NAME")]
        public string FullName { get; set; }

        /// <summary>
        /// 编码.
        /// </summary>
        [SugarColumn(ColumnName = "F_EN_CODE")]
        public string EnCode { get; set; }

        /// <summary>
        /// 拼音.
        /// </summary>
        [SugarColumn(ColumnName = "F_SIMPLE_SPELLING")]
        public string SimpleSpelling { get; set; }

        /// <summary>
        /// 默认.
        /// </summary>
        [SugarColumn(ColumnName = "F_IS_DEFAULT")]
        public int? IsDefault { get; set; }

        /// <summary>
        /// 描述.
        /// </summary>
        [SugarColumn(ColumnName = "F_DESCRIPTION")]
        public string Description { get; set; }

        /// <summary>
        /// 类别主键.
        /// </summary>
        [SugarColumn(ColumnName = "F_DICTIONARY_TYPE_ID")]
        public string DictionaryTypeId { get; set; }
    }
}
