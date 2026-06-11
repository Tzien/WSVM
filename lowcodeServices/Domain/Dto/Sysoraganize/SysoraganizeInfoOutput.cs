using System;

namespace CeriOS.示例.Entitys.Dto.Sysoraganize;
 
/// <summary>
/// AddNewTableTest输出参数.
/// </summary>
public class SysoraganizeInfoOutput
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
    public int? Sort { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    public int Status { get; set; }

}