using CeriOS.示例.Entitys.Dto.Dictbaseinfo;

namespace CeriOS.示例.Entitys.Dto.Mediainfo;

/// <summary>
/// 测试等于查询更新输入.
/// </summary>
public class MediainfoUpInput : MediainfoCrInput
{
    /// <summary>
    /// 主键.
    /// </summary>
    public string? id { get; set; }
}