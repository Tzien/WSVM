using JNPF.Common.Models;
using Newtonsoft.Json;

using System;
namespace CeriOS.示例.Entitys.Dto.Pictable;

/// <summary>
/// 校验图片上传功能详情输出参数.
/// </summary>
public class PictableDetailOutput
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