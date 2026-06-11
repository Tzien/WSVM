using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    public class DbSelectDto
    {
        public string DbType { get; set; }
        public List<DbSelectItemDto> Items { get; set; }
    }

    public class DbSelectItemDto
    {
        public string DbConfigId { get; set; }
        public string Name { get; set; }
    }
}
