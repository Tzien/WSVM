using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IFormCategoryRepository
    {

        Task<QueryResponseDto<List<FormCategory>>> GetFormCategoryAsync(int pageIndex, int pageSize);

        Task<bool> InsertFormCategoryAsync(FormCategory parameter);
        Task<FormCategory> GetFormCategoryByIdAsync(string id);
        Task<bool> UpdateFormCategoryAsync(FormCategory parameter);
        Task<bool> DeleteFormCategoryAsync(string id);
        Task<List<FormCategory>> GetFormCategoriesByIds(List<string> ids);
    }

}