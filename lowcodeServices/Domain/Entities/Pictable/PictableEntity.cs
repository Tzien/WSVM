using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 校验图片上传功能实体.
/// </summary>
[SugarTable("pictable")]
public class PictableEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "PicURL")]
    public string? PicURL { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "FileURL")]
    public string? FileURL { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}