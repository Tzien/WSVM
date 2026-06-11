using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    /// <summary>
    /// 数据库表信息输出.
    /// </summary>
    public class DatabaseTableInfoOutput
    {
        public string linkId { get; set; }
        /// <summary>
        /// 表信息.
        /// </summary>
        public TableInfoOutput tableInfo { get; set; }

        /// <summary>
        /// 表字段.
        /// </summary>
        public List<TableFieldOutput> tableFieldList { get; set; }

        /// <summary>
        /// 表数据.
        /// </summary>
        public bool hasTableData { get; set; }
    }

    public class aaa
    {
        public string linkId { get; set; }
       
        public TableInfoOutput tableInfo { get; set; }
        public List<TableFieldOutput> tableFieldList { get; set; }

        public bool hasTableData { get; set; }
    }

    public class TableInfoOutput
    {
        /// <summary>
        /// 旧表名称.
        /// </summary>
        public string table { get; set; }

        /// <summary>
        /// 新表名称.
        /// </summary>
        public string newTable { get; set; }

        /// <summary>
        /// 表说明.
        /// </summary>
        public string tableName { get; set; }
    }


    public class TableFieldOutput
    {
        /// <summary>
        /// 是否主键（0：是，1：否）.
        /// </summary>
        public int primaryKey { get; set; }

        /// <summary>
        /// 是否允许为空.
        /// </summary>
        public int allowNull { get; set; }

        /// <summary>
        /// 长度.
        /// </summary>
        public int dataLength { get; set; }

        /// <summary>
        /// 类型.
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 列名.
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段注释.
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 是否自增（0：是，1：否）.
        /// </summary>
        public int identity { get; set; }
    }
}
