using JNPF.Common.Filter;
using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace CeriOS.示例.Entitys.Dto.Dictbaseinfo;

/// <summary>
/// 2测试代码生成Demo2列表查询输入.
/// </summary>
public class DictbaseinfoListQueryInput : PageInputBase
{
    [JsonExtensionData]
    public Dictionary<string, JsonElement> Extra { get; set; }

    /// <summary>
    /// 高级查询.
    /// </summary>
    public string superQueryJson { get; set; }

    /// <summary>
    /// 选择导出数据ids.
    /// </summary>
    public string selectIds { get; set; }

    /// <summary>
    /// 选择导出数据key.
    /// </summary>
    public string selectKey { get; set; }

    /// <summary>
    /// 导出类型.
    /// </summary>
    public int dataType { get; set; }

    /// <summary>
    /// 关键词查询.
    /// </summary>
    public string jnpfKeyword { get; set; }

    /// <summary>
    /// 允许编辑.
    /// </summary>
    public string AllowEdit { get; set; }

    /// <summary>
    /// 所属子系统.
    /// </summary>
    public object BelongSystem { get; set; }

    /// <summary>
    /// 创建者名称.
    /// </summary>
    public string CreateName { get; set; }

    /// <summary>
    /// 创建时间.
    /// </summary>
    public List<string> CreateTime { get; set; }

    /// <summary>
    /// 字典类型.
    /// </summary>
    public string DictType { get; set; }

    /// <summary>
    /// 字段小数.
    /// </summary>
    public string FieldDecimal { get; set; }

}