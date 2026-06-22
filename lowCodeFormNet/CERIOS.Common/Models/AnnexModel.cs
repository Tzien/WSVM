
namespace CERIOS.Common.Models
{
    /// <summary>
    /// 附件模型
    /// 版 本：V1.0.0
    /// 版 权：数科平台
    /// 作 者：开发平台组-田自恩
    /// </summary>
    public class AnnexModel
    {
        /// <summary>
        /// 文件ID.
        /// </summary>
        public string? FileId { get; set; }

        /// <summary>
        /// 文件名称.
        /// </summary>
        public string? FileName { get; set; }

        /// <summary>
        /// 文件大小.
        /// </summary>
        public string? FileSize { get; set; }

        /// <summary>
        /// 文件上传时间.
        /// </summary>
        public DateTime FileTime { get; set; }

        /// <summary>
        /// 文件状态.
        /// </summary>
        public string? FileState { get; set; }

        /// <summary>
        /// 文件类型.
        /// </summary>
        public string? FileType { get; set; }
    }
}
