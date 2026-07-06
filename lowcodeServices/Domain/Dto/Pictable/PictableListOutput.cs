using System;
using System.Collections.Generic;
using JNPF.Common.Models;
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
    public List<FileControlsModel> PicURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> FileURL { get; set; }

}