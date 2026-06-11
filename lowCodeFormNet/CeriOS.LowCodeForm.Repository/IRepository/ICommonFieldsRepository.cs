using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.Repository.IRepository
{
public interface ICommonFieldsRepository
{
    
Task<QueryResponseDto<List<CommonFields>>> GetCommonFieldsAsync(string? ColumnName, int pageIndex, int pageSize);
    
    Task<bool> InsertCommonFieldsAsync(CommonFields parameter);
    Task<CommonFields> GetCommonFieldsByIdAsync(string id);
    Task<bool> UpdateCommonFieldsAsync(CommonFields parameter);
    Task<bool> DeleteCommonFieldsAsync(string id);
        Task<QueryByIdResponseDto<List<CommonFields>>> GetFieldsXiaLaAsync();
}

}