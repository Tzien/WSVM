using CeriOS.Core.Model;
using CeriOS.LowCodeForm.Model.Model;
using SqlSugar;

namespace CeriOS.LowCodeForm.Model.ViewModel
{

    public class FormDbDto1
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string? FormDbId { get; set; }
        public string? FormDesignId { get; set; }
        /// <summary>
        /// 表单编码
        /// </summary>
        public string? Code { get; set; }
        /// <summary>
        /// 表单分类id
        /// </summary>
        public string? FormCategoryId { get; set; }
        public string? CategoryName { get; set; }
        /// <summary>
        /// 数据库id
        /// </summary>
        public string? DbId { get; set; }
        public string? TableName { get; set; }
        public string? PK { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>
        public string? Name { get; set; }
        /// <summary>
        /// 表单设计
        /// </summary>
        public string? FormJson { get; set; }
        /// <summary>
        /// 说明
        /// </summary>
        public string? Remark { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 列表设计
        /// </summary>
        public string? TableJson { get; set; }
        public bool? Status { get; set; }
        /// <summary>
        /// 主表id
        /// </summary>
        public string? FormId { get; set; }
        public string? FK { get; set; }
    }


    public class FormDbDto : PublicProperty
    {
        /// <summary>
        /// 主键
        /// </summary>
        public string? FormDesignId { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>

        public string? Name { get; set; }


        [SugarColumn(IsIgnore = true)]
        public string? FullName { get; set; }
        /// <summary>
        /// 编码
        /// </summary>

        public string? Code { get; set; }

        [SugarColumn(IsIgnore = true)]
        public string? enCode { get; set; }
        /// <summary>
        /// 分类
        /// </summary>

        public string? FormCategoryId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? WebType { get; set; }
        /// <summary>
        /// 排序
        /// </summary>
        public int? Sort { get; set; }
        /// <summary>
        /// 说明
        /// </summary>

        public string? Remark { get; set; }



        /// <summary>
        /// 数据库id
        /// </summary>
        public string? DbId { get; set; }


        public string? FormJson { get; set; }


        public string? TableJson { get; set; }
        public int? Status { get; set; }
        /// <summary>
        /// 列表JSON包.
        /// </summary>
        public string? ColumnData { get; set; }

        /// <summary>
        /// App列表JSON包.
        /// </summary>
        public string? AppColumnData { get; set; }
        public string? ColumnDataStr { get; set; }
        public string? AppColumnDataStr { get; set; }

        public List<FormDb> FormDbs { get; set; }
    }
}