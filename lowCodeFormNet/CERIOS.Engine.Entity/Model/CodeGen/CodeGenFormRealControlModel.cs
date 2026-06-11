using CERIOS.DependencyInjection;

namespace CERIOS.Engine.Entity.Model.CodeGen;

/// <summary>
/// 代码生成表单真实控件.
/// </summary>
[SuppressSniffer]
public class CodeGenFormRealControlModel
{
    public string ceriKey { get; set; }

    public string vModel { get; set; }

    public bool multiple { get; set; }

    public List<CodeGenFormRealControlModel> children { get; set; }
}