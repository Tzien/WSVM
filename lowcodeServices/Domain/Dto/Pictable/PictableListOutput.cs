using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.Pictable;

/// <summary>
/// 校验图片上传功能输入参数.
/// </summary>
public class PictableListOutput
{
    /// <summary>
    /// .
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public object? PicURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public object? FileURL { get; set; }

}