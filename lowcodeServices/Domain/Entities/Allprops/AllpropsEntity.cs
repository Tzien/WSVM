using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试其他模板2实体.
/// </summary>
[SugarTable("allprops")]
public class AllpropsEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Enabled")]
    public byte? Enabled { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Sort")]
    public int? Sort { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}