using CeriOS.LowCodeForm.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    public class TestApiRequestDto
    {
        public List<ApiParam> ApiParams { get; set; } = new List<ApiParam>();
        public List<ApiDataParams> SqlApiParams { get; set; } = new List<ApiDataParams>();
    }
    public class ApiParam
    {
        public string name { get; set; }
        public string value { get; set; }
    }

}
