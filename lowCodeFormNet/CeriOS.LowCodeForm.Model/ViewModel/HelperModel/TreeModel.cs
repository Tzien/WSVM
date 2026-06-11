using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.HelperModel
{

    /// <summary>
    /// 树模型基类.
    /// </summary>
    public class TreeModel
    {
        /// <summary>
        /// 获取节点id.
        /// </summary>
        /// <returns></returns>
        public string id { get; set; }

        /// <summary>
        /// 获取节点父id.
        /// </summary>
        /// <returns></returns>
        public string parentId { get; set; }

        /// <summary>
        /// 是否有子级.
        /// </summary>
        public bool hasChildren { get; set; }

        /// <summary>
        /// 设置Children.
        /// </summary>
        public List<object>? children { get; set; } = new List<object>();

        /// <summary>
        /// 子节点数量.
        /// </summary>
        public int num { get; set; }

        /// <summary>
        /// 是否为子节点.
        /// </summary>
        public bool? isLeaf { get; set; } = false;

        /// <summary>
        /// 是否禁用.
        /// </summary>
        public bool disabled { get; set; } = false;
    }
}
