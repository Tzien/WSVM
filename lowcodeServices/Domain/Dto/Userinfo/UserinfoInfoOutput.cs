using System;

namespace CeriOS.示例.Entitys.Dto.Userinfo;
 
/// <summary>
/// 测试表单设计绑定列关系输出参数.
/// </summary>
public class UserinfoInfoOutput
{
    /// <summary>
    /// 主键ID.
    /// </summary>
    public string? id { get; set; }

    /// <summary>
    /// 是否激活.
    /// </summary>
    public string Enabled { get; set; }

    /// <summary>
    /// 用户名称.
    /// </summary>
    public string? RealName { get; set; }

    /// <summary>
    /// 备注.
    /// </summary>
    public string? Remark { get; set; }

    /// <summary>
    /// 性别[0:男，1:女].
    /// </summary>
    public int Sex { get; set; }

}