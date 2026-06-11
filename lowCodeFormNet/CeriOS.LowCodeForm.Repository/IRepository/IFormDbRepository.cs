using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IFormDbRepository
    {

        Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync(int pageIndex, int pageSize);
        Task<bool> InsertFormDesignAsync(FormDesign parameter);
        Task<bool> UpdateFormDesignAsync(FormDesign parameter);
        Task<bool> DeleteFormDesign(string id);

        Task<bool> InsertFormDbAsync(FormDb parameter);
        Task<bool> InsertRange(List<FormDb> parameters);
        Task<FormDb> GetFormDbByIdAsync(string id);
        Task<bool> UpdateFormDbAsync(FormDb parameter);
        Task<bool> DeleteFormDbAsync(string formDesignId);
        Task<FormDesign> GetFormDesignById(string id);
        Task<QueryResponseDto<FormDbDto>> GetFormDesignDtoByIdAsync(string id);
        Task<FormDb> GetFormDbByNameAsync(string name);
    }

}