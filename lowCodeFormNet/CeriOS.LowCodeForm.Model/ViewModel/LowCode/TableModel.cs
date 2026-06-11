using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    /// <summary>
    /// 在线开发模型数据表模型
    /// </summary>
    public class TableModel : DbTableRelationModel
    {
        /// <summary>
        /// 控件key.
        /// </summary>
        public string ControlKey { get; set; }

        /// <summary>
        /// 列字段.
        /// </summary>
        public List<EntityFieldModel> fields { get; set; }
    }
}
