using JNPF.Common.Models;
using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.Testallfunction;
 
/// <summary>
/// 第三版修改输入参数.
/// </summary>
public class TestallfunctionCrInput
{
    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> FileURL { get; set; }

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