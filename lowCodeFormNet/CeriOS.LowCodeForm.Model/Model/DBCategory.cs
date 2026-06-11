using SqlSugar;
using CeriOS.Core.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Model.Model
{

public class DBCategory : PublicProperty 
{
    
     [SugarColumn(IsNullable = true, Length = 100)]
    public string? Name { get; set; }
    
     [SugarColumn(IsPrimaryKey = true, Length = 36)]
    public string DBCategoryId { get; set; }
    }
}