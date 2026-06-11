using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Model.ViewModel.HelperModel
{
    public static class CodeGenHelper
    {
        public static string ConvertDataType(string dataType)
        {
            switch (dataType.ToLower())
            {
                case "text":
                case "longtext":
                case "varchar":
                case "char":
                case "nvarchar":
                case "nchar":
                case "timestamp":
                case "string":
                    return "string?";

                case "int":
                case "smallint":
                    return "int?";

                case "tinyint":
                    return "byte?";

                case "bigint":
                // sqlite数据库
                case "integer":
                    return "long";

                case "bit":
                    return "bool";

                case "money":
                case "smallmoney":
                case "numeric":
                case "decimal":
                    return "decimal?";

                case "real":
                    return "Single?";

                case "datetime":
                case "datetime2":
                case "smalldatetime":
                case "date":
                    return "DateTime?";

                case "float":
                case "double":
                    return "double?";

                case "image":
                case "binary":
                case "varbinary":
                    return "byte[]";

                case "uniqueidentifier":
                    return "Guid";

                default:
                    return "object";
            }
        }

    }
}
