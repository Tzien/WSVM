using System;

namespace CeriOS.示例.Entitys.Dto.Sysshiftclass;
 
/// <summary>
/// 班组和班组人员设计修改输入参数.
/// </summary>
public class SysshiftclassCrInput
{
    /// <summary>
    /// 名称.
    /// </summary>
    public string? ModuleName { get; set; }

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
    /// 是否以删除.
    /// </summary>
    public int? IsDeleted { get; set; }

}