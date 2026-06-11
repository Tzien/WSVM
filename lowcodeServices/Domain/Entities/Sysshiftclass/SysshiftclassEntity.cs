using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 班组和班组人员设计实体.
/// </summary>
[SugarTable("sysshiftclass")]
public class SysshiftclassEntity
{
    /// <summary>
    /// 主键id.
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    [SugarColumn(ColumnName = "ModuleName")]
    public string? ModuleName { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 编码.
    /// </summary>
    [SugarColumn(ColumnName = "Code")]
    public string? Code { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    [SugarColumn(ColumnName = "Sort")]
    public int? Sort { get; set; }

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