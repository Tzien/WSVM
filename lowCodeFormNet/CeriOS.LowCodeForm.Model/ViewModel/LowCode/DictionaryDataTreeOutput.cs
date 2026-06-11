using CeriOS.LowCodeForm.Model.ViewModel.HelperModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    public class DictionaryDataTreeOutput : TreeModel
    {
        /// <summary>
        /// 项目名称.
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 项目编码.
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// 状态(1-可用,0-禁用).
        /// </summary>
        public int enabledMark { get; set; }

        /// <summary>
        /// 排序.
        /// </summary>
        public long? sortCode { get; set; }
    }
}
