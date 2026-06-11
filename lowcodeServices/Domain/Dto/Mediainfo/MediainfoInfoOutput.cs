using System;

namespace CeriOS.示例.Entitys.Dto.Mediainfo;
 
/// <summary>
/// 测试等于查询输出参数.
/// </summary>
public class MediainfoInfoOutput
{
    /// <summary>
    /// 主键.
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// 名称.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// 排序.
    /// </summary>
    public int? Sort { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 是否以删除.
    /// </summary>
    public int IsDeleted { get; set; }

}