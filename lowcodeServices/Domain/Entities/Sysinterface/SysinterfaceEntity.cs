using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试等于查询2实体.
/// </summary>
[SugarTable("sysinterface")]
public class SysinterfaceEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 系统ID.
    /// </summary>
    [SugarColumn(ColumnName = "SysInfoID")]
    public string? SysInfoID { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [SugarColumn(ColumnName = "Sort")]
    public int? Sort { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    [SugarColumn(ColumnName = "IsActive")]
    public byte? IsActive { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}