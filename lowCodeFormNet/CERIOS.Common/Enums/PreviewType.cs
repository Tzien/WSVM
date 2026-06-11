using System.ComponentModel;

namespace CERIOS.Common.Enums;

/// <summary>
/// 文件预览方式.
/// </summary>
public enum PreviewType
{
    /// <summary>
    /// KKfile.
    /// </summary>
    [Description("KKfile")]
    kkfile = 0,

    /// <summary>
    /// 永中.
    /// </summary>
    [Description("yozo")]
    yozo = 1,
}