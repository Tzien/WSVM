using CeriOS.Core.Model;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.Model2
{
    public class Signature : PublicProperty
    {
        [SugarColumn(IsPrimaryKey = true, Length = 36)]
        public string SignatureId { get; set; }
        [SugarColumn(IsNullable = true, Length = 100)]
        public string? Name { get; set; }
        [SugarColumn(IsNullable = true, Length = 100)]
        public string? Code { get; set; }
        [SugarColumn(IsNullable = true, Length = 1000)]
        public string? UserIds { get; set; }

        [SugarColumn(IsNullable = true, Length = 4000)]
        public string? Img { get; set; }

    }
}
