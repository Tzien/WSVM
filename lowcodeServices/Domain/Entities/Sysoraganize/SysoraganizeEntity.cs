using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// AddNewTableTest实体.
/// </summary>
[SugarTable("sysoraganize")]
public class SysoraganizeEntity
{
    /// <summary>
    /// 主键id.
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

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
    /// 是否激活.
    /// </summary>
    [SugarColumn(ColumnName = "Status")]
    public byte? Status { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}