using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.BasicApi.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
namespace CeriOS.LowCodeForm.BasicApi.Repository.Repository
{
    public class CommonFieldsRepository : ICommonFieldsRepository
    {
        public Repository<CommonFields> _db { get; set; }
    
        public CommonFieldsRepository()
        {
            _db = new Repository<CommonFields>();
        }
        
public async Task<QueryResponseDto<List<CommonFields>>> GetCommonFieldsAsync(string? ColumnName, int pageIndex, int pageSize)
{
    QueryResponseDto<List<CommonFields>> queryResponseDto = new QueryResponseDto<List<CommonFields>>();
    var query = _db.Context.Queryable<CommonFields>().Where(o => !o.IsDeleted);
                      if (!string.IsNullOrEmpty(ColumnName))
                {
                                  query = query.Where(o => o.ColumnName.Contains(ColumnName));
                                }
    
    queryResponseDto.Total = await query.CountAsync();
    queryResponseDto.Data = await query.OrderBy(o => o.ColumnName)
        .ToPageListAsync(pageIndex, pageSize);
        queryResponseDto.OK(true,"查询成功");
    return queryResponseDto;
}

        public async Task<QueryByIdResponseDto<List<CommonFields>>> GetFieldsXiaLaAsync()
        {
            QueryByIdResponseDto<List<CommonFields>> queryResponseDto = new QueryByIdResponseDto<List<CommonFields>>();
            queryResponseDto.OK(true, "查询成功");
            queryResponseDto.Data = (await _db.GetListAsync(o => !o.IsDeleted)).OrderBy(p => p.ColumnName).ToList();
            return queryResponseDto;
        }


        public async Task<bool> InsertCommonFieldsAsync(CommonFields parameter)
        {
            return await _db.InsertAsync(parameter);
        }

         public async Task<bool> UpdateCommonFieldsAsync(CommonFields parameter)
        {
            return await _db.UpdateAsync(parameter);
        }
                public async Task<bool> DeleteCommonFieldsAsync(string id)
        {
        int num = 0;
                            num = await _db.Context.Updateable<CommonFields>()
                .Where(o => o.cId == id)
                .SetColumns(o => new CommonFields { IsDeleted = true })
                .ExecuteCommandAsync();
                               return num > 0;
        }

    
        // 根据ID查询
        public async Task<CommonFields> GetCommonFieldsByIdAsync(string id)
        {
            return await _db.GetByIdAsync(id);
        }
    }
}