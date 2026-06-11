using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试等于查询实体.
/// </summary>
[SugarTable("mediainfo")]
public class MediainfoEntity
{
    /// <summary>
    /// 主键.
    /// </summary>
    [SugarColumn(ColumnName = "MediaInfoId", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [SugarColumn(ColumnName = "Name")]
    public string? Name { get; set; }

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
    /// 是否以删除.
    /// </summary>
    [SugarColumn(ColumnName = "IsDeleted")]
    public byte? IsDeleted { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}