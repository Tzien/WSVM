using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 测试表单设计绑定列关系实体.
/// </summary>
[SugarTable("userinfo")]
public class UserinfoEntity
{
    /// <summary>
    /// 主键ID.
    /// </summary>
    [SugarColumn(ColumnName = "Id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    [SugarColumn(ColumnName = "Enabled")]
    public byte? Enabled { get; set; }

    /// <summary>
    /// 用户名称.
    /// </summary>
    [SugarColumn(ColumnName = "RealName")]
    public string? RealName { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    [SugarColumn(ColumnName = "Remark")]
    public string? Remark { get; set; }

    /// <summary>
    /// 性别[0:男，1:女].
    /// </summary>
    [SugarColumn(ColumnName = "Sex")]
    public int? Sex { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}