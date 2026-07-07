using CERIOS.Common.Enums;
using CERIOS.ConfigurableOptions.Options;

namespace CERIOS.Common.Options
{
    public sealed class AppOptions : IConfigurableOptions
    {
        /// <summary>
        /// 系统文件路径.
        /// </summary>
        public string SystemPath { get; set; }

        /// <summary>
        /// 允许图片类型.
        /// </summary>
        public List<string> AllowUploadImageType { get; set; }

        /// <summary>
        /// 允许上传文件类型.
        /// </summary>
        public List<string> AllowUploadFileType { get; set; }

        /// <summary>
        /// 微信允许上传文件类型.
        /// </summary>
        public List<string> WeChatUploadFileType { get; set; }

        /// <summary>
        /// 过滤上传文件名称特殊字符.
        /// </summary>
        public List<string> SpecialString { get; set; }

        /// <summary>
        /// 文件预览方式.
        /// </summary>
        public PreviewType PreviewType { get; set; }

        /// <summary>
        /// 后端外网域名.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// KKFileView 文件预览服务地址.
        /// </summary>
        public string KKFileDomain { get; set; }

        /// <summary>
        /// 过滤内网ip.
        /// </summary>
        public List<string> InternalNetwork { get; set; }

        /// <summary>
        /// 永中 配置.
        /// </summary>
        public YOZO YOZO { get; set; }
    }

    /// <summary>
    /// 永中.
    /// </summary>
    public class YOZO
    {
        /// <summary>
        /// 域名.
        /// </summary>
        public string Domain { get; set; }

        /// <summary>
        /// 域名key.
        /// </summary>
        public string domainKey { get; set; }

        /// <summary>
        /// 上传接口.
        /// </summary>
        public string UploadAPI { get; set; }

        /// <summary>
        /// 预览接口.
        /// </summary>
        public string DownloadAPI { get; set; }

        /// <summary>
        /// 应用Id.
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 签名.
        /// </summary>
        public string AppKey { get; set; }
    }
}