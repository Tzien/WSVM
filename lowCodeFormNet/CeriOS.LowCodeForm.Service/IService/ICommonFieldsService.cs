using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.Service.IService{



public interface ICommonFieldsService
{

  Task<QueryResponseDto<List<CommonFields>>> GetCommonFieldsAsync(string? ColumnName, int pageIndex, int pageSize);
        Task<ResponseDto> InsertCommonFieldsAsync(CommonFields parameter);
    Task<ResponseDto> UpdateCommonFieldsAsync(CommonFields parameter);
        
    Task<ResponseDto> DeleteCommonFieldsAsync(string id);
    Task<QueryByIdResponseDto<CommonFields>> GetCommonFieldsByIdAsync(string id);
        Task<QueryByIdResponseDto<List<CommonFields>>> GetFieldsXiaLaAsync();
}

}