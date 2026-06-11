using SqlSugar;
using CeriOS.Core.Model;
namespace CeriOS.LowCodeForm.Model.Model
{

public class FormCategory : PublicProperty 
{
    
     [SugarColumn(IsNullable = true, Length = 100)]
    public string? Name { get; set; }
    
     [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public string FormCategoryId { get; set; }
    }
}