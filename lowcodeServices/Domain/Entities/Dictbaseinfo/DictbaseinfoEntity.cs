using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 2测试代码生成Demo2实体.
/// </summary>
[SugarTable("dictbaseinfo")]
public class DictbaseinfoEntity
{
    /// <summary>
    /// 允许编辑.
    /// </summary>
    [SugarColumn(ColumnName = "AllowEdit")]
    public byte? AllowEdit { get; set; }

    /// <summary>
    /// 所属子系统.
    /// </summary>
    [SugarColumn(ColumnName = "BelongSystem")]
    public string? BelongSystem { get; set; }

    /// <summary>
    /// 创建者名称.
    /// </summary>
    [SugarColumn(ColumnName = "CreateName")]
    public string? CreateName { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    [SugarColumn(ColumnName = "CreateTime")]
    public DateTime? CreateTime { get; set; }

    /// <summary>
    /// 主键id.
    /// </summary>
    [SugarColumn(ColumnName = "DictBaseInfoId", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 字典类型.
    /// </summary>
    [SugarColumn(ColumnName = "DictType")]
    public string? DictType { get; set; }

    /// <summary>
    /// 字段小数.
    /// </summary>
    [SugarColumn(ColumnName = "FieldDecimal")]
    public int? FieldDecimal { get; set; }

    /// <summary>
    /// 说明.
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}