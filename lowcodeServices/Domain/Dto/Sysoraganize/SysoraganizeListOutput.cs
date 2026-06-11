using System;

namespace CeriOS.示例.Entitys.Dto.Sysoraganize;

/// <summary>
/// AddNewTableTest输入参数.
/// </summary>
public class SysoraganizeListOutput
{
    /// <summary>
    /// 主键id.
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 编码.
    /// </summary>
    public string? Code { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    public string Sort { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    public string Status { get; set; }

}