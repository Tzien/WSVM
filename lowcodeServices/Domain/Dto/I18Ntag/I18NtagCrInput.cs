using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.I18Ntag;
 
/// <summary>
/// 测试其他模板修改输入参数.
/// </summary>
public class I18NtagCrInput
{
    /// <summary>
    /// .
    /// </summary>
    public string? TagName { get; set; }

    /// <summary>
    /// 0 通用标签、各个菜单内的元素  1 系统/菜单标签 .
    /// </summary>
    public int? Code { get; set; }

}