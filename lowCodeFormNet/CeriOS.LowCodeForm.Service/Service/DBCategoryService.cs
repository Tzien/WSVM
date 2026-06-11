using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.Service.Service
{

    public class DBCategoryService : IDBCategoryService
    {
        public IDBCategoryRepository _repository { get; set; }
    
        public DBCategoryService(IDBCategoryRepository repository)
        {
            _repository = repository;
        }      
         
        
public async Task<QueryResponseDto<List<DBCategory>>> GetDBCategoryAsync( int pageIndex, int pageSize)
{
return  await _repository.GetDBCategoryAsync( pageIndex,  pageSize);
}

public async Task<ResponseDto> InsertDBCategoryAsync(DBCategory parameter)
        {
                            parameter.DBCategoryId = Guid.NewGuid().ToString("N");
                                       
            var result = await _repository.InsertDBCategoryAsync(parameter);
            return new ResponseDto().OK(result,result ? "添加成功":"添加失败");
        }
        
        public async Task<ResponseDto> UpdateDBCategoryAsync(DBCategory parameter)
        {
             var result = await _repository.UpdateDBCategoryAsync(parameter);
             return new ResponseDto().OK(result,result ? "修改成功":"修改失败");
        }
        
        
        public async Task<ResponseDto> DeleteDBCategoryAsync(string id)
        {
            var result = await _repository.DeleteDBCategoryAsync(id);
            return new ResponseDto().OK(result,result ? "删除成功":"删除失败");
        }
    
        // 根据ID查询
        public async Task<QueryByIdResponseDto<DBCategory>> GetDBCategoryByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<DBCategory>();
            dto.Data = await _repository.GetDBCategoryByIdAsync(id);
            dto.OK(true,"查询成功");
            return dto;
        }
    }

}