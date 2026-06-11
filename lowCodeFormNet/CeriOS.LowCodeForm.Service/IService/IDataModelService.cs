using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.IService
{
    public interface IDataModelService
    {
        Task<ResponseDto> GetDBTables(string linkId, string? tableName, int pageIndex, int pageSize);
        Task<List<DBConfig>> GetDBXia();
        Task<ResponseDto> AddTable(DatabaseTableInfoOutput databaseTableInfoOutput);
        Task<ResponseDto> deltable(string linkId, string table);
        Task<ResponseDto> GetTableAsync(string linkId, string tableName);
        Task<ResponseDto> UpdateTable(DatabaseTableInfoOutput databaseTableInfoOutput);
    }
}
