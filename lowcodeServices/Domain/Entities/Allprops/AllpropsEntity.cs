using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试编辑模板其他控件实体.
/// </summary>
[SugarTable("allprops")]
public class AllpropsEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Color")]
    public string? Color { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "CreateTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Enabled")]
    public byte? Enabled { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Enmu")]
    public int? Enmu { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "FWB")]
    public string? FWB { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "HK")]
    public int? HK { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "ID", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "LastLoginTime")]
    public DateTime? LastLoginTime { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Name")]
    public string? Name { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "PF")]
    public int? PF { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Text")]
    public string? Text { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}