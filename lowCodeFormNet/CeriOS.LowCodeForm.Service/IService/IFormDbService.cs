using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Service.IService
{



    public interface IFormDbService
    {

        Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync(int pageIndex, int pageSize);
        Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync2(int pageIndex, int pageSize);
        Task<QueryByIdResponseDto<string>> InsertFormDbAsync(FormDbDto parameter);
        Task<ResponseDto> UpdateFormDbAsync(FormDbDto parameter);

        Task<ResponseDto> DeleteFormDbAsync(string id);
        Task<QueryByIdResponseDto<FormDb>> GetFormDbByIdAsync(string id);
        Task<QueryByIdResponseDto<FormDb>> GetFormDbByNameAsync(string name);
        Task<QueryByIdResponseDto<FormDesign>> GetFormDesignByIdAsync(string id);
        Task<QueryResponseDto<FormDbDto>> GetFormDesignDtoByIdAsync(string id);
    }

}