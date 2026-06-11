using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
namespace CeriOS.LowCodeForm.Service.IService
{



    public interface IFormCategoryService
    {

        Task<QueryResponseDto<List<FormCategory>>> GetFormCategoryAsync(int pageIndex, int pageSize);
        Task<ResponseDto> InsertFormCategoryAsync(FormCategory parameter);
        Task<ResponseDto> UpdateFormCategoryAsync(FormCategory parameter);

        Task<ResponseDto> DeleteFormCategoryAsync(string id);
        Task<QueryByIdResponseDto<FormCategory>> GetFormCategoryByIdAsync(string id);
        Task<List<FormCategory>> GetFormCategoriesByIds(List<string> ids);
    }

}