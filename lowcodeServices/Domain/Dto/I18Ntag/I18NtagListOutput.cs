using System;

namespace CeriOS.示例.Entitys.Dto.I18Ntag;

/// <summary>
/// 测试表单控件2输入参数.
/// </summary>
public class I18NtagListOutput
{
    /// <summary>
    /// .
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// 0 通用标签、各个菜单内的元素  1 系统/菜单标签 .
    /// </summary>
    public string Code { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    public string CreateTime { get; set; }

}