using CeriOS.Core.Common.Tree;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.BasicApi.Service.IService;
using CeriOS.LowCodeForm.BasicApi.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
namespace CeriOS.LowCodeForm.BasicApi.Service.Service
{

    public class CommonFieldsService : ICommonFieldsService
    {
        public ICommonFieldsRepository _repository { get; set; }
    
        public CommonFieldsService(ICommonFieldsRepository repository)
        {
            _repository = repository;
        }      
         
        
public async Task<QueryResponseDto<List<CommonFields>>> GetCommonFieldsAsync(string? ColumnName, int pageIndex, int pageSize)
{
return  await _repository.GetCommonFieldsAsync(ColumnName, pageIndex,  pageSize);
}

public async Task<ResponseDto> InsertCommonFieldsAsync(CommonFields parameter)
        {
                            parameter.cId = Guid.NewGuid().ToString("N");
                                       
            var result = await _repository.InsertCommonFieldsAsync(parameter);
            return new ResponseDto().OK(result,result ? "添加成功":"添加失败");
        }
        
        public async Task<ResponseDto> UpdateCommonFieldsAsync(CommonFields parameter)
        {
             var result = await _repository.UpdateCommonFieldsAsync(parameter);
             return new ResponseDto().OK(result,result ? "修改成功":"修改失败");
        }
        
        
        public async Task<ResponseDto> DeleteCommonFieldsAsync(string id)
        {
            var result = await _repository.DeleteCommonFieldsAsync(id);
            return new ResponseDto().OK(result,result ? "删除成功":"删除失败");
        }
    
        // 根据ID查询
        public async Task<QueryByIdResponseDto<CommonFields>> GetCommonFieldsByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<CommonFields>();
            dto.Data = await _repository.GetCommonFieldsByIdAsync(id);
            dto.OK(true,"查询成功");
            return dto;
        }

        public async Task<QueryByIdResponseDto<List<CommonFields>>> GetFieldsXiaLaAsync()
        {
            return await _repository.GetFieldsXiaLaAsync();
        }
    }

}