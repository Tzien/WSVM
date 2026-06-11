using SqlSugar;

namespace Domain.Entities;

[SugarTable("User")]
public class User
{
    [SugarColumn(IsPrimaryKey = true)]
    public string Id { get; set; }


    [SugarColumn(Length = 100)]
    public string Name { get; set; }
}
