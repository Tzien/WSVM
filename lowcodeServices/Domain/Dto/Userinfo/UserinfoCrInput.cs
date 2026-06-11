using System;

namespace CeriOS.示例.Entitys.Dto.Userinfo;
 
/// <summary>
/// 测试表单设计绑定列关系修改输入参数.
/// </summary>
public class UserinfoCrInput
{
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
    public string Sex { get; set; }

}