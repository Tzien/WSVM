using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    /// <summary>
    /// 插槽模型
    /// </summary>
    public class SlotModel
    {
        /// <summary>
        /// 前.
        /// </summary>
        public string prepend { get; set; }

        /// <summary>
        /// 后.
        /// </summary>
        public string append { get; set; }

        /// <summary>
        /// 默认.
        /// </summary>
        public string @default { get; set; }
    }
}
