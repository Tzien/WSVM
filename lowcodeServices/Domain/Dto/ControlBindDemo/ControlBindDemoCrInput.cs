using JNPF.Common.Models;
using System;
using System.Collections.Generic;

namespace CeriOS.示例.Entitys.Dto.ControlBindDemo;
 
/// <summary>
/// 控件修改输入参数.
/// </summary>
public class ControlBindDemoCrInput
{
    /// <summary>
    /// 级联选择显示文本.
    /// </summary>
    public List<string> cascade_text { get; set; }

    /// <summary>
    /// 日期选择值（年月日）.
    /// </summary>
    public DateTime? date_selected { get; set; }

    /// <summary>
    /// 日期时间选择值（精确到秒）.
    /// </summary>
    public DateTime? datetime_selected { get; set; }

    /// <summary>
    /// 文件上传URL/路径.
    /// </summary>
    public List<FileControlsModel> file_url { get; set; }

    /// <summary>
    /// 多图片存储，格式：[{"url":"路径","name":"名称","size":12345,"thumbnail":"缩略图路径"}].
    /// </summary>
    public List<FileControlsModel> images { get; set; }

    /// <summary>
    /// 颜色值，如 #FF5500.
    /// </summary>
    public string? color_value { get; set; }

    /// <summary>
    /// 评分值，范围1-5或1-10.
    /// </summary>
    public byte? rating_value { get; set; }

    /// <summary>
    /// 滑块整数值.
    /// </summary>
    public int? slider_int_value { get; set; }

    /// <summary>
    /// 富文本内容（HTML格式）.
    /// </summary>
    public string? rich_text_content { get; set; }

}