using CERIOS.Engine.Entity.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    /// <summary>
    /// 列表页各配置基类.
    /// </summary>
    public class IndexEachConfigBase : FieldsModel
    {
        /// <summary>
        /// 字段.
        /// </summary>
        public string prop { get; set; }

        /// <summary>
        /// 列名.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 标题名I18nCode.
        /// </summary>
        public string labelI18nCode { get; set; }

        /// <summary>
        /// 控件KEY.
        /// </summary>
        public string ceriKey { get; set; }
    }
}
