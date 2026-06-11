using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.Controller
{

[Route("api/[controller]")]
    [ApiController]
public class FormCategoryController : ControllerBase
{
    public IFormCategoryService _FormCategoryService;

        public FormCategoryController(IFormCategoryService FormCategoryService)
        {

             _FormCategoryService = FormCategoryService;
        }
           
        [HttpGet]
        [Route("GetFormCategoryAsync")]
                  public async Task<QueryResponseDto<List<FormCategory>>> GetFormCategoryAsync( int pageIndex, int pageSize)
        {
            return await _FormCategoryService.GetFormCategoryAsync( pageIndex,  pageSize);
        }
      
            [HttpPost]
        [Route("InsertFormCategoryAsync")]
        public async Task<ResponseDto> InsertFormCategoryAsync(FormCategory parameter)
        {
            return await _FormCategoryService.InsertFormCategoryAsync(parameter);
        }
        [HttpPost]
        [Route("UpdateFormCategoryAsync")]
        public async Task<ResponseDto> UpdateFormCategoryAsync(FormCategory parameter)
        {
            return await _FormCategoryService.UpdateFormCategoryAsync(parameter);
        }
              
        [HttpGet]
        [Route("DeleteFormCategoryAsync")]
        public async Task<ResponseDto> DeleteFormCategoryAsync(string id)
        {
            return await _FormCategoryService.DeleteFormCategoryAsync(id);
        }

        [HttpGet]
        [Route("GetFormCategoryByIdAsync")]
        public async Task<QueryByIdResponseDto<FormCategory>> GetFormCategoryByIdAsync(string id)
        {
            return await _FormCategoryService.GetFormCategoryByIdAsync(id);
        }

}

}