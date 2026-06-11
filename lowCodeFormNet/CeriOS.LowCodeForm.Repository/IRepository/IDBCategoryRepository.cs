using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Repository.IRepository
{
public interface IDBCategoryRepository
{
    
Task<QueryResponseDto<List<DBCategory>>> GetDBCategoryAsync( int pageIndex, int pageSize);
    
    Task<bool> InsertDBCategoryAsync(DBCategory parameter);
    Task<DBCategory> GetDBCategoryByIdAsync(string id);
    Task<bool> UpdateDBCategoryAsync(DBCategory parameter);
    Task<bool> DeleteDBCategoryAsync(string id);
}

}