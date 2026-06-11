using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Service.IService
{



    public interface IDBConfigService
    {

        Task<QueryResponseDto<List<DBConfig>>> GetDBConfigAsync(int pageIndex, int pageSize, string? name, string? dbType);
        Task<ResponseDto> InsertDBConfigAsync(DBConfig parameter);
        Task<ResponseDto> UpdateDBConfigAsync(DBConfig parameter);

        Task<ResponseDto> DeleteDBConfigAsync(string id);
        Task<QueryByIdResponseDto<DBConfig>> GetDBConfigByIdAsync(string id);
        Task<string> ExecuteSql(string id,string sql);
        Task<bool> TestDb(DBConfig data);
        Task<QueryResponseDto<List<DbSelectDto>>> GetDbSelect();
        Task<List<TableInfo>> GetAllTables(string dbConfigId, string? tableName);
    }

}