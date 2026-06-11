using System;

namespace CeriOS.示例.Entitys.Dto.I18Ntag;
 
/// <summary>
/// 测试表单控件2输出参数.
/// </summary>
public class I18NtagInfoOutput
{
    /// <summary>
    /// .
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    public string? TagName { get; set; }

    /// <summary>
    /// 0 通用标签、各个菜单内的元素  1 系统/菜单标签 .
    /// </summary>
    public int Code { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    public DateTime? CreateTime { get; set; }

}