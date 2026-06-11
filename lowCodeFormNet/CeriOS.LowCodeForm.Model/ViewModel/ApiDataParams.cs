using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel
{
    public class ApiDataParams
    {

        public string paramName { get; set; }
        public string paramType { get; set; }
        public object defaultValue { get; set; }
        public bool required { get; set; }
    }
    public class FieldList
    {

        public string Name { get; set; }
        public string MapName { get; set; }
    }
}
