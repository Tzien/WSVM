using SqlSugar;
using System;

namespace CeriOS.示例.Entitys;

/// <summary>
/// 控件实体.
/// </summary>
[SugarTable("control_bind_demo")]
public class ControlBindDemoEntity
{
    /// <summary>
    /// 主键ID.
    /// </summary>
    [SugarColumn(ColumnName = "id", IsPrimaryKey = true)]
    public string? id { get; set; }

    /// <summary>
    /// 级联选择显示文本.
    /// </summary>
    [SugarColumn(ColumnName = "cascade_text")]
    public string? cascade_text { get; set; }

    /// <summary>
    /// 日期选择值（年月日）.
    /// </summary>
    [SugarColumn(ColumnName = "date_selected")]
    public DateTime? date_selected { get; set; }

    /// <summary>
    /// 日期时间选择值（精确到秒）.
    /// </summary>
    [SugarColumn(ColumnName = "datetime_selected")]
    public DateTime? datetime_selected { get; set; }

    /// <summary>
    /// 文件上传URL/路径.
    /// </summary>
    [SugarColumn(ColumnName = "file_url")]
    public string? file_url { get; set; }

    /// <summary>
    /// 多图片存储，格式：[{"url":"路径","name":"名称","size":12345,"thumbnail":"缩略图路径"}].
    /// </summary>
    [SugarColumn(ColumnName = "images")]
    public string? images { get; set; }

    /// <summary>
    /// 颜色值，如 #FF5500.
    /// </summary>
    [SugarColumn(ColumnName = "color_value")]
    public string? color_value { get; set; }

    /// <summary>
    /// 评分值，范围1-5或1-10.
    /// </summary>
    [SugarColumn(ColumnName = "rating_value")]
    public byte? rating_value { get; set; }

    /// <summary>
    /// 滑块整数值.
    /// </summary>
    [SugarColumn(ColumnName = "slider_int_value")]
    public int? slider_int_value { get; set; }

    /// <summary>
    /// 富文本内容（HTML格式）.
    /// </summary>
    [SugarColumn(ColumnName = "rich_text_content")]
    public string? rich_text_content { get; set; }

  

    /// <summary>
    /// 创建者名称Id.
    /// </summary>
    [SugarColumn(ColumnName = "CreateId")]
    public string? CreateId { get; set; }
}