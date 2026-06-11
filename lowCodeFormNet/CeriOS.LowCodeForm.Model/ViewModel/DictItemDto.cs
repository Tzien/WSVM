using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    [SugarTable("DictDetail")]
    public class DictItemDto
    {
        public string DictDetailId { get; set; }
        public string ItemName{ get; set; }
    }
}
