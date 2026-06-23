using JNPF.Common.Models;
using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.Pictable;
 
/// <summary>
/// 校验图片上传功能修改输入参数.
/// </summary>
public class PictableCrInput
{
    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> PicURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public List<FileControlsModel> FileURL { get; set; }

}