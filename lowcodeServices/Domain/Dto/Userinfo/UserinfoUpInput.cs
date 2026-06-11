using CeriOS.示例.Entitys.Dto.Dictbaseinfo;

namespace CeriOS.示例.Entitys.Dto.Userinfo;

/// <summary>
/// 测试表单设计绑定列关系更新输入.
/// </summary>
public class UserinfoUpInput : UserinfoCrInput
{
    /// <summary>
    /// 主键ID.
    /// </summary>
    public string? id { get; set; }
}