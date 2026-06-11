using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.LowCode
{
    public class ModelDataConfigOutput
    {
        /// <summary>
        /// 主键.
        /// </summary>
        public string id { get; set; }

        /// <summary>
        /// 表单JSON包.
        /// </summary>
        public string formData { get; set; }

        /// <summary>
        /// 列表JSON包.
        /// </summary>
        public string columnData { get; set; }

        /// <summary>
        /// 列表JSON包.
        /// </summary>
        public string appColumnData { get; set; }

        /// <summary>
        /// 工作流编码.
        /// </summary>
        public string flowEnCode { get; set; }

        /// <summary>
        /// 工作流引擎ID.
        /// </summary>
        public string flowId { get; set; }

        /// <summary>
        /// 工作流模板JSON.
        /// </summary>
        public string flowTemplateJson { get; set; }

        /// <summary>
        /// 名称.
        /// </summary>
        public string fullName { get; set; }

        /// <summary>
        /// 类型
        /// 1-自定义表单，2-系统表单.
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 页面类型（1、纯表单，2、表单加列表，3、表单列表工作流）.
        /// </summary>
        public string webType { get; set; }

        /// <summary>
        /// 编码.
        /// </summary>
        public string enCode { get; set; }

        /// <summary>
        /// Web地址.
        /// </summary>
        public string urlAddress { get; set; }

        /// <summary>
        /// App地址.
        /// </summary>
        public string appUrlAddress { get; set; }

        /// <summary>
        /// 数据接口id.
        /// </summary>
        public string interfaceId { get; set; }

        /// <summary>
        /// 存储字段列表.
        /// </summary>
        public List<PropsValueModel> propsValueList { get; set; } = new List<PropsValueModel>();
    }
}
