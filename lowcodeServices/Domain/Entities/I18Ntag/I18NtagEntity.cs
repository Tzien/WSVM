using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试其他模板实体.
/// </summary>
[SugarTable("i18ntag")]
public class I18NtagEntity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "TagName")]
    public string? TagName { get; set; }

    /// <summary>
    /// 0 通用标签、各个菜单内的元素  1 系统/菜单标签 .
    /// </summary>
    [SugarColumn(ColumnName = "Code")]
    public int? Code { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}