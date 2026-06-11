using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CERIOS.Engine.Entity.Model
{
    public class ButtonConfigModel
    {
        /// <summary>
        /// 值.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 图标.
        /// </summary>
        public string icon { get; set; }

        /// <summary>
        /// 标签.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 标签 I18nCode.
        /// </summary>
        public string labelI18nCode { get; set; }

        /// <summary>
        /// 是否显示.
        /// </summary>
        public bool show { get; set; } = true;
    }


    /// <summary>
    /// 子表按钮.
    /// </summary>
    public class ChildTableBtnsList
    {
        /// <summary>
        /// 按钮key.
        /// </summary>
        public string value { get; set; }

        /// <summary>
        /// 按钮label.
        /// </summary>
        public string label { get; set; }

        /// <summary>
        /// 按钮label I18nCode.
        /// </summary>
        public string labelI18nCode { get; set; }

        /// <summary>
        /// 是否显示.
        /// </summary>
        public bool show { get; set; }

        /// <summary>
        /// 按钮类型.
        /// </summary>
        public string btnType { get; set; }

        /// <summary>
        /// Icon.
        /// </summary>
        public string btnIcon { get; set; }

        /// <summary>
        /// showConfirm.
        /// </summary>
        public string showConfirm { get; set; }

        /// <summary>
        /// 动作配置.
        /// </summary>
        public object actionConfig { get; set; }

        public int actionType { get; set; }
    }

}
