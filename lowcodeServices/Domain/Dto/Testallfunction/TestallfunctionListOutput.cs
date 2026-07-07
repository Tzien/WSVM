using System;
using System.Collections.Generic;
using JNPF.Common.Models;
namespace CeriOS.示例.Entitys.Dto.Testallfunction;

/// <summary>
/// 第三版输入参数.
/// </summary>
public class TestallfunctionListOutput
{
    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> FileURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> PictureURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public string? Remark { get; set; }

}