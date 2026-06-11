using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using CeriOS.LowCodeForm.Service.IService;
using Mapster;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;

namespace CeriOS.LowCodeForm.BasicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DataModelController : ControllerBase
    {
        private readonly IDataModelService _dataModelService;

        public DataModelController(IDataModelService dataModelService) 
        {
            _dataModelService = dataModelService;
        }  

        [HttpGet("GetDBTables")]
        public async Task<ResponseDto> GetDBTables(string linkId, string? tableName, int pageIndex, int pageSize)
        { 
        return await _dataModelService.GetDBTables(linkId, tableName,  pageIndex,  pageSize);
        }

        [HttpGet("GetTableAsync")]
        public async Task<ResponseDto> GetTableAsync(string linkId, string? tableName)
        {
            return await _dataModelService.GetTableAsync(linkId, tableName);
        }

        [HttpGet("GetDBXia")]
        public async Task<QueryByIdResponseDto<List<DBConfig>>> GetDBXia()
        {
            var str = new QueryByIdResponseDto<List<DBConfig>>();
            str.Data =  await _dataModelService.GetDBXia();
            str.OK(true, "");
            return str;
        }

        [HttpPost("AddTable")]
        public async Task<ResponseDto> AddTable(DatabaseTableInfoOutput databaseTableInfoOutput)
        {
            return await _dataModelService.AddTable(databaseTableInfoOutput); 
        }

        [HttpGet("DelTable")]
        public async Task<ResponseDto> DelTable(string linkId,string table)
        {
            return await _dataModelService.deltable(linkId, table);
        }


        [HttpPost("UpdateTable")]
        public async Task<ResponseDto> UpdateTable(DatabaseTableInfoOutput databaseTableInfoOutput)
        {
            return await _dataModelService.UpdateTable(databaseTableInfoOutput);
        }
    }

    public class SystemMapper : IRegister
    {
        public void Register(TypeAdapterConfig config)
        {
            config.ForType<DbColumnInfo, DbTableFieldModel>()
            .Map(dest => dest.field, src => src.DbColumnName)
            .Map(dest => dest.fieldName, src => src.ColumnDescription)
            .Map(dest => dest.dataLength, src => src.Length)
            .Map(dest => dest.identity, src => src.IsIdentity ? true : false)
            .Map(dest => dest.primaryKey, src => src.IsPrimarykey ? 1 : 0)
            .Map(dest => dest.allowNull, src => src.IsNullable ? 1 : 0)
            .Map(dest => dest.decimalDigits, src => src.DecimalDigits)
            .Map(dest => dest.dataType, src => src.DataType.ToLower())
            .Map(dest => dest.defaults, src => src.DefaultValue);
            config.ForType<DbTableFieldModel, DbColumnInfo>()
                .Map(dest => dest.DbColumnName, src => src.field)
                .Map(dest => dest.ColumnDescription, src => src.fieldName)
                .Map(dest => dest.Length, src => src.dataLength)
                .Map(dest => dest.IsIdentity, src => src.identity)
                .Map(dest => dest.IsPrimarykey, src => src.primaryKey)
                .Map(dest => dest.IsNullable, src => src.allowNull == 1)
                .Map(dest => dest.DecimalDigits, src => src.decimalDigits)
                .Map(dest => dest.DefaultValue, src => src.defaults);
            config.ForType<DbTableInfo, DbTableModel>()
            .Map(dest => dest.table, src => src.Name)
            .Map(dest => dest.tableName, src => src.Description);
            config.ForType<DbTableInfo, TableInfoOutput>()
                .Map(dest => dest.table, src => src.Name)
                .Map(dest => dest.tableName, src => src.Description);
            config.ForType<DbColumnInfo, TableFieldOutput>()
            .Map(dest => dest.field, src => src.DbColumnName)
            .Map(dest => dest.fieldName, src => src.ColumnDescription)
            .Map(dest => dest.dataLength, src => src.Length)
            .Map(dest => dest.primaryKey, src => src.IsPrimarykey ? 1 : 0)
            .Map(dest => dest.identity, src => src.IsIdentity ? 1 : 0)
            .Map(dest => dest.allowNull, src => src.IsNullable ? 1 : 0);
        }
    }
}
  