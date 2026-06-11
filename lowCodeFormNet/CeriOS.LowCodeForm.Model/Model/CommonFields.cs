using SqlSugar;
using CeriOS.Core.Model;
namespace CeriOS.LowCodeForm.Model.Model
{

    public class CommonFields : PublicProperty
    {

        [SugarColumn(IsNullable = true, IsPrimaryKey = false, Length = 50)]
        public string? ColumnName { get; set; }

        [SugarColumn(IsNullable = true, IsPrimaryKey = false, Length = 50)]
        public string? Remarks { get; set; }
        [SugarColumn(IsPrimaryKey = false)]
        public bool? IsEmpty { get; set; }

        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string cId { get; set; }

        [SugarColumn(IsNullable = true, IsPrimaryKey = false, Length = 50)]
        public string? Type { get; set; }
        [SugarColumn(IsNullable = true, IsPrimaryKey = false, Length = 50)]
        public int? Length { get; set; }
    }
}