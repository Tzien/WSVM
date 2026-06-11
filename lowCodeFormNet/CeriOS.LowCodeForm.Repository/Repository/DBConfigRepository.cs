using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class DBConfigRepository : IDBConfigRepository
    {
        public Repository<DBConfig> _db { get; set; }

        public DBConfigRepository()
        {
            _db = new Repository<DBConfig>();
        }

        public async Task<QueryResponseDto<List<DBConfig>>> GetDBConfigAsync(int pageIndex, int pageSize,string? name,string? dbType)
        {
            QueryResponseDto<List<DBConfig>> queryResponseDto = new QueryResponseDto<List<DBConfig>>();
            var query = _db.Context.Queryable<DBConfig>().Where(o => !o.IsDeleted && (string.IsNullOrEmpty(name)|| o.Name.Contains(name))
            && (string.IsNullOrEmpty(dbType) || o.DbType == dbType));

            queryResponseDto.Total = await query.CountAsync();
            queryResponseDto.Data = await query.OrderBy(o => o.Sort)
                .ToPageListAsync(pageIndex, pageSize);
            queryResponseDto.OK(true, "查询成功");
            return queryResponseDto;
        }



        public async Task<bool> InsertDBConfigAsync(DBConfig parameter)
        {
            return await _db.InsertAsync(parameter);
        }

        public async Task<bool> UpdateDBConfigAsync(DBConfig parameter)
        {
            return await _db.UpdateAsync(parameter);
        }
        public async Task<bool> DeleteDBConfigAsync(string id)
        {
            int num = 0;
            num = await _db.Context.Updateable<DBConfig>()
.Where(o => o.DBConfigId == id)
.SetColumns(o => new DBConfig { IsDeleted = true })
.ExecuteCommandAsync();
            return num > 0;
        }


        // 根据ID查询
        public async Task<DBConfig> GetDBConfigByIdAsync(string id)
        {
            return await _db.GetByIdAsync(id);
        }
    }
}