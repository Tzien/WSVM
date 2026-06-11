using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class DBCategoryRepository : IDBCategoryRepository
    {
        public Repository<DBCategory> _db { get; set; }
    
        public DBCategoryRepository()
        {
            _db = new Repository<DBCategory>();
        }
        
public async Task<QueryResponseDto<List<DBCategory>>> GetDBCategoryAsync( int pageIndex, int pageSize)
{
    QueryResponseDto<List<DBCategory>> queryResponseDto = new QueryResponseDto<List<DBCategory>>();
    var query = _db.Context.Queryable<DBCategory>().Where(o => !o.IsDeleted);
      
    queryResponseDto.Total = await query.CountAsync();
    queryResponseDto.Data = await query
        .ToPageListAsync(pageIndex, pageSize);
        queryResponseDto.OK(true,"查询成功");
    return queryResponseDto;
}



        public async Task<bool> InsertDBCategoryAsync(DBCategory parameter)
        {
            return await _db.InsertAsync(parameter);
        }

         public async Task<bool> UpdateDBCategoryAsync(DBCategory parameter)
        {
            return await _db.UpdateAsync(parameter);
        }
                public async Task<bool> DeleteDBCategoryAsync(string id)
        {
        int num = 0;
                            num = await _db.Context.Updateable<DBCategory>()
                .Where(o => o.DBCategoryId == id)
                .SetColumns(o => new DBCategory { IsDeleted = true })
                .ExecuteCommandAsync();
                               return num > 0;
        }

    
        // 根据ID查询
        public async Task<DBCategory> GetDBCategoryByIdAsync(string id)
        {
            return await _db.GetByIdAsync(id);
        }
    }
}