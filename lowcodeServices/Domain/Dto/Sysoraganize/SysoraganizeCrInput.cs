using System;

namespace CeriOS.示例.Entitys.Dto.Sysoraganize;
 
/// <summary>
/// AddNewTableTest修改输入参数.
/// </summary>
public class SysoraganizeCrInput
{
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
    public int? Sort { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    public int? Status { get; set; }

}