using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    /// <summary>
    /// 实体字段模型
    /// </summary>
    public class EntityFieldModel
    {
        /// <summary>
        /// 字段名称.
        /// </summary>
        public string field { get; set; }

        /// <summary>
        /// 字段说明.
        /// </summary>
        public string fieldName { get; set; }

        /// <summary>
        /// 数据类型.
        /// </summary>
        public string dataType { get; set; }

        /// <summary>
        /// 数据长度.
        /// </summary>
        public string dataLength { get; set; }

        /// <summary>
        /// 主键.
        /// </summary>
        public int? primaryKey { get; set; }

        /// <summary>
        /// 可空.
        /// </summary>
        public int? allowNull { get; set; }

        /// <summary>
        /// 自增.
        /// </summary>
        public int? identity { get; set; }
    }
}
