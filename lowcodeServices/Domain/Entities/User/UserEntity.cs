using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 演示示例Demo实体.
/// </summary>
[SugarTable("user")]
public class UserEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Address")]
    public string? Address { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}