using JNPF.Common.Filter;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CeriOS.示例.Entitys.Dto.Mediainfo;

/// <summary>
/// 测试等于查询列表查询输入.
/// </summary>
public class MediainfoListQueryInput : PageInputBase
{
     [JsonExtensionData]
     public Dictionary<string, JsonElement> Extra { get; set; }

    /// <summary>
    /// 选择导出数据ids.
    /// </summary>-
    public string selectIds { get; set; }

    /// <summary>
    /// 选择导出数据key.
    /// </summary>
    public string selectKey { get; set; }

    /// <summary>
    /// 导出类型.
    /// </summary>
    public int dataType { get; set; }
}