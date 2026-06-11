using SqlSugar;
using CeriOS.Core.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Model.Model
{

    public class FormDb : PublicProperty
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string FormDbId { get; set; }
        [SugarColumn(IsNullable = true,  Length = 36)]
        public string? FormDesignId { get; set; }
        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? TableName { get; set; }

        [SugarColumn(IsNullable = true,  Length = 100)]
        public int? TypeId { get; set; }

        [SugarColumn(IsNullable = true,  Length = 36)]
        public string? PK { get; set; }


    

        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? FK { get; set; }



        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? Fields { get; set; }

    }

    /// <summary>
    /// 基础设计
    /// </summary>
    public class FormDesign : PublicProperty
    {
        /// <summary>
        /// 主键
        /// </summary>
        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string? FormDesignId { get; set; }
        /// <summary>
        /// 表单名称
        /// </summary>

        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? Name { get; set; }
        /// <summary>
        /// 编码
        /// </summary>

        [SugarColumn(IsNullable = true,  Length = 100)]
        public string? Code { get; set; }
        /// <summary>
        /// 分类
        /// </summary>

        [SugarColumn(IsNullable = true,  Length = 36)]
        public string? FormCategoryId { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? Sort { get; set; }
        /// <summary>
        /// 说明
        /// </summary>

        [SugarColumn(IsNullable = true,  Length = 200)]
        public string? Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [SugarColumn(IsNullable = true)]
        public int? WebType { get; set; }

        /// <summary>
        /// 数据库id
        /// </summary>
        [SugarColumn(IsNullable = true,  Length = 50)]
        public string? DbId { get; set; }


        [SugarColumn(IsNullable = true,ColumnDataType = "clob" )]
        public string? FormJson { get; set; }


        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? TableJson { get; set; }
        [SugarColumn(IsNullable = true)]
        public int? Status { get; set; }

        /// <summary>
        /// 列表JSON包.
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? ColumnData { get; set; }

        /// <summary>
        /// App列表JSON包.
        /// </summary>
        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? AppColumnData { get; set; }


        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? ColumnDataStr { get; set; }
        [SugarColumn(IsNullable = true, ColumnDataType = "clob")]
        public string? AppColumnDataStr { get; set; }
        
        //public int? Type { get; set; }
    }
}