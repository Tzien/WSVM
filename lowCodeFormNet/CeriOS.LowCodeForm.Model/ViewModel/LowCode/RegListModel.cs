using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    public class RegListModel
    {
        /// <summary>
        /// 正则表达式.
        /// </summary>
        public string pattern { get; set; }

        /// <summary>
        /// 错误提示.
        /// </summary>
        public string message { get; set; }

        /// <summary>
        /// 错误提示 I18nCode.
        /// </summary>
        public string messageI18nCode { get; set; }
    }
}
