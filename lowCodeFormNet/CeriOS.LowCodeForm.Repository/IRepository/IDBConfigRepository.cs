using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IDBConfigRepository
    {

        Task<QueryResponseDto<List<DBConfig>>> GetDBConfigAsync(int pageIndex, int pageSize, string? name, string? dbType);

        Task<bool> InsertDBConfigAsync(DBConfig parameter);
        Task<DBConfig> GetDBConfigByIdAsync(string id);
        Task<bool> UpdateDBConfigAsync(DBConfig parameter);
        Task<bool> DeleteDBConfigAsync(string id);
    }

}