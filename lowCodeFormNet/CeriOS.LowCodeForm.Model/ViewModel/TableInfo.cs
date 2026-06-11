using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    public class TableInfo
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<ColumnInfo> ColumnInfos { get; set; }
    }


    public class ColumnInfo
    {
        public string Name { get; set; }
        public string DataType { get; set; }
        public string Description { get; set; }
        public bool IsNullable { get; set; }
        public long? Length { get; set; }

        public bool IsPrimaryKey { get; set; }  // 新增主键标识
        /// <summary>
        /// 命名规范(重命名).
        /// </summary>
        public string? ReName { get; set; }
    }
}
