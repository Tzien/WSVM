using JNPF.Common.Models;

using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.Testallfunction;
 
/// <summary>
/// 第四版输出参数.
/// </summary>
public class TestallfunctionInfoOutput
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

    /// <summary>
    /// .
    /// </summary>
    public int? Sort { get; set; }

}