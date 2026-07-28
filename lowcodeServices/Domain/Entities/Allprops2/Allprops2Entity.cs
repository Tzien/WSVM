using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试编辑模板实体.
/// </summary>
[SugarTable("allprops2")]
public class Allprops2Entity
{
    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "DanHang")]
    public string? DanHang { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "DanXuan")]
    public string? DanXuan { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "DuoHang")]
    public string? DuoHang { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "DuoXuan")]
    public string? DuoXuan { get; set; }

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
    [SugarColumn(ColumnName = "JiLian")]
    public string? JiLian { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "KaiGuan")]
    public byte? KaiGuan { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "PF")]
    public int? PF { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "RiQi")]
    public DateTime? RiQi { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "ShiJian")]
    public string? ShiJian { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "ShuZi")]
    public int? ShuZi { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "XiaLa")]
    public string? XiaLa { get; set; }

    /// <summary>
    /// .
    /// </summary>
    [SugarColumn(ColumnName = "YanSe")]
    public string? YanSe { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}