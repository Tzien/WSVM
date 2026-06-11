using CeriOS.Core.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.Model
{
    public class DataApi : PublicProperty
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string DataApiId { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Name { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Code { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Category { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string? CategoryName { get; set; }

        /// <summary>
        /// 0：静态数据，1：SQL，2：API
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public string? Type { get; set; }

        [SugarColumn(IsNullable = true)]
        public int? Sort { get; set; }

        [SugarColumn(ColumnName = "DESCSTR",IsNullable = true)]
        public string? Desc { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? StaticData { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? DbConfigId { get; set; }
        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? SqlStr { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? DataHandler { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? ExVer { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? ApiUrl { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? Method { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? BodyJsonData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? BodyXmlData { get; set; }

        [SugarColumn(IsNullable = true)]
        public string? BodyType { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? HeaderData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? QueryData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? FormData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? WwwFormData { get; set; }

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? ApiDataParamsJson { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<ApiDataParams> ApiDataParams { get; set; } = new List<ApiDataParams>();

        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? FieldListJson { get; set; }
        [SugarColumn(IsIgnore = true)]
        public List<FieldList> FieldList { get; set; } = new List<FieldList>();
    }

}
