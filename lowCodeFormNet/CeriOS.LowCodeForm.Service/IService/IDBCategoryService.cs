using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Service.IService{



public interface IDBCategoryService
{

  Task<QueryResponseDto<List<DBCategory>>> GetDBCategoryAsync( int pageIndex, int pageSize);
        Task<ResponseDto> InsertDBCategoryAsync(DBCategory parameter);
    Task<ResponseDto> UpdateDBCategoryAsync(DBCategory parameter);
        
    Task<ResponseDto> DeleteDBCategoryAsync(string id);
    Task<QueryByIdResponseDto<DBCategory>> GetDBCategoryByIdAsync(string id);
}

}