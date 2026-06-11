using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CeriOS.LowCodeForm.BasicApi.Service.IService;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.BasicApi.Controller
{

[Route("api/[controller]")]
    [ApiController]
public class CommonFieldsController : ControllerBase
{
    public ICommonFieldsService _CommonFieldsService;

        public CommonFieldsController(ICommonFieldsService CommonFieldsService)
        {

             _CommonFieldsService = CommonFieldsService;
        }
           
        [HttpGet]
        [Route("GetCommonFieldsAsync")]
                  public async Task<QueryResponseDto<List<CommonFields>>> GetCommonFieldsAsync(string? ColumnName, int pageIndex, int pageSize)
        {
            return await _CommonFieldsService.GetCommonFieldsAsync(ColumnName, pageIndex,  pageSize);           
        }
      
            [HttpPost]
        [Route("InsertCommonFieldsAsync")]
        public async Task<ResponseDto> InsertCommonFieldsAsync(CommonFields parameter)
        {
            return await _CommonFieldsService.InsertCommonFieldsAsync(parameter);
        }
        [HttpPost]
        [Route("UpdateCommonFieldsAsync")]
        public async Task<ResponseDto> UpdateCommonFieldsAsync(CommonFields parameter)
        {
            return await _CommonFieldsService.UpdateCommonFieldsAsync(parameter);
        }
              
        [HttpGet]
        [Route("DeleteCommonFieldsAsync")]
        public async Task<ResponseDto> DeleteCommonFieldsAsync(string id)
        {
            return await _CommonFieldsService.DeleteCommonFieldsAsync(id);
        }

        [HttpGet]
        [Route("GetCommonFieldsByIdAsync")]
        public async Task<QueryByIdResponseDto<CommonFields>> GetCommonFieldsByIdAsync(string id)
        {
            return await _CommonFieldsService.GetCommonFieldsByIdAsync(id);
        }

        [HttpGet]
        [Route("GetFieldsXiaLaAsync")]
        public async Task<QueryByIdResponseDto<List<CommonFields>>> GetFieldsXiaLaAsync()
        {
            return await _CommonFieldsService.GetFieldsXiaLaAsync();
        }

    }

}