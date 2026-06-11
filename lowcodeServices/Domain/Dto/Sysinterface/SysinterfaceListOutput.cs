using System;

namespace CeriOS.示例.Entitys.Dto.Sysinterface;

/// <summary>
/// 测试等于查询2输入参数.
/// </summary>
public class SysinterfaceListOutput
{
    /// <summary>
    /// .
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// 系统ID.
    /// </summary>
    public string? SysInfoID { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    public string Sort { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    public string IsActive { get; set; }

}