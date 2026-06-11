using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.Core.Model.ViewModel;
namespace CeriOS.LowCodeForm.Service.Service
{

    public class FormCategoryService : IFormCategoryService
    {
        public IFormCategoryRepository _repository { get; set; }

        public FormCategoryService(IFormCategoryRepository repository)
        {
            _repository = repository;
        }


        public async Task<QueryResponseDto<List<FormCategory>>> GetFormCategoryAsync(int pageIndex, int pageSize)
        {
            return await _repository.GetFormCategoryAsync(pageIndex, pageSize);
        }

        public async Task<ResponseDto> InsertFormCategoryAsync(FormCategory parameter)
        {
            parameter.FormCategoryId = Guid.NewGuid().ToString("N");

            var result = await _repository.InsertFormCategoryAsync(parameter);
            return new ResponseDto().OK(result, result ? "添加成功" : "添加失败");
        }

        public async Task<ResponseDto> UpdateFormCategoryAsync(FormCategory parameter)
        {
            var result = await _repository.UpdateFormCategoryAsync(parameter);
            return new ResponseDto().OK(result, result ? "修改成功" : "修改失败");
        }


        public async Task<ResponseDto> DeleteFormCategoryAsync(string id)
        {
            var result = await _repository.DeleteFormCategoryAsync(id);
            return new ResponseDto().OK(result, result ? "删除成功" : "删除失败");
        }

        // 根据ID查询
        public async Task<QueryByIdResponseDto<FormCategory>> GetFormCategoryByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<FormCategory>();
            dto.Data = await _repository.GetFormCategoryByIdAsync(id);
            dto.OK(true, "查询成功");
            return dto;
        }

        public async Task<List<FormCategory>> GetFormCategoriesByIds(List<string> ids)
        {
            return await _repository.GetFormCategoriesByIds(ids);
        }
    }

}