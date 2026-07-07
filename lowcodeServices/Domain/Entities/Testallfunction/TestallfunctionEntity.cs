using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 第三版实体.
/// </summary>
[SugarTable("testallfunction")]
public class TestallfunctionEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "FileURL")]
    public string? FileURL { get; set; }

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
    [SugarColumn(ColumnName = "PictureURL")]
    public string? PictureURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}