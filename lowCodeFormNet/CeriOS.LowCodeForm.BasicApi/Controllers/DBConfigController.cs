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
    public class DBConfigController : ControllerBase
    {
        public IDBConfigService _dbConfigService;

        public DBConfigController(IDBConfigService dbConfigService)
        {

            _dbConfigService = dbConfigService;
        }

        [HttpGet]
        [Route("GetDBConfigAsync")]
        public async Task<QueryResponseDto<List<DBConfig>>> GetDBConfigAsync(int pageIndex, int pageSize,string? name="",string? dbType="")
        {
            return await _dbConfigService.GetDBConfigAsync(pageIndex, pageSize, name, dbType);
        }

        [HttpPost]
        [Route("InsertDBConfigAsync")]
        public async Task<ResponseDto> InsertDBConfigAsync(DBConfig parameter)
        {
            return await _dbConfigService.InsertDBConfigAsync(parameter);
        }
        [HttpPost]
        [Route("UpdateDBConfigAsync")]
        public async Task<ResponseDto> UpdateDBConfigAsync(DBConfig parameter)
        {
            return await _dbConfigService.UpdateDBConfigAsync(parameter);
        }

        [HttpGet]
        [Route("DeleteDBConfigAsync")]
        public async Task<ResponseDto> DeleteDBConfigAsync(string id)
        {
            return await _dbConfigService.DeleteDBConfigAsync(id);
        }

        [HttpGet]
        [Route("GetDBConfigByIdAsync")]
        public async Task<QueryByIdResponseDto<DBConfig>> GetDBConfigByIdAsync(string id)
        {
            return await _dbConfigService.GetDBConfigByIdAsync(id);
        }

        /// <summary>
        /// 测试连接
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("TestDb")]
        public async Task<QueryByIdResponseDto<bool>> TestDb(DBConfig data)
        {
            var result = await _dbConfigService.TestDb(data);
            return new QueryByIdResponseDto<bool>() { Code = 200,Data=result, Success = true };
        }



        /// <summary>
        /// 获取数据库连接串下拉框数据源
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("GetDbSelect")]
        public async Task<QueryResponseDto<List<DbSelectDto>>> GetDbSelect()
        {
            return await _dbConfigService.GetDbSelect();
        }



        /// <summary>
        /// 获取数据库表信息
        /// </summary>
        /// <param name="dbConfigId">数据库配置id</param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetAllTables")]
        public async Task<QueryResponseDto<List<TableInfo>>> GetAllTables(string dbConfigId, string? tableName)
        {
            try
            {
                var result = await _dbConfigService.GetAllTables(dbConfigId, tableName);
                return new QueryResponseDto<List<TableInfo>>() { Code = 200, Success = true, Data = result };
            }
            catch (Exception ex)
            {
                return new QueryResponseDto<List<TableInfo>>() { Code = 200, Success = false, Message=ex.Message };
            }
        }
    }

}


