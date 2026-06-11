using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.Controller
{

[Route("api/[controller]")]
    [ApiController]
public class DBCategoryController : ControllerBase
{
    public IDBCategoryService _DBCategoryService;

        public DBCategoryController(IDBCategoryService DBCategoryService)
        {

             _DBCategoryService = DBCategoryService;
        }
           
        [HttpGet]
        [Route("GetDBCategoryAsync")]
                  public async Task<QueryResponseDto<List<DBCategory>>> GetDBCategoryAsync( int pageIndex, int pageSize)
        {
            return await _DBCategoryService.GetDBCategoryAsync( pageIndex,  pageSize);
        }
      
            [HttpPost]
        [Route("InsertDBCategoryAsync")]
        public async Task<ResponseDto> InsertDBCategoryAsync(DBCategory parameter)
        {
            return await _DBCategoryService.InsertDBCategoryAsync(parameter);
        }
        [HttpPost]
        [Route("UpdateDBCategoryAsync")]
        public async Task<ResponseDto> UpdateDBCategoryAsync(DBCategory parameter)
        {
               return await _DBCategoryService.UpdateDBCategoryAsync(parameter);
        }
              
        [HttpGet]
        [Route("DeleteDBCategoryAsync")]
        public async Task<ResponseDto> DeleteDBCategoryAsync(string id)
        {
            return await _DBCategoryService.DeleteDBCategoryAsync(id);
        }

        [HttpGet]
        [Route("GetDBCategoryByIdAsync")]
        public async Task<QueryByIdResponseDto<DBCategory>> GetDBCategoryByIdAsync(string id)
        {
            return await _DBCategoryService.GetDBCategoryByIdAsync(id);
        }

}

}