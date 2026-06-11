using CeriOS.Core.Common.DB;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Repository.IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class VisualPersonalRepository : IVisualPersonalRepository
    {
        public Repository<VisualPersonalEntity> repository { get; set; }
        public VisualPersonalRepository(Repository<VisualPersonalEntity> repository) { this.repository = repository; }
        public async Task<List<VisualPersonalEntity>> GetListBySth(string menuId, string userId)
        {
            return await repository.Context.Queryable<VisualPersonalEntity>()
                .Where(it => it.IsDeleted == false && it.MenuId == menuId && it.CreateId == userId)
                .OrderBy(a => a.CreateTime, OrderByType.Desc).ToListAsync();
        }

        public async Task<int> AddView(VisualPersonalEntity entity)
        {
            return await repository.Context.Insertable(entity).ExecuteCommandAsync();
        }

        public async Task<int> GetByIdSth(string id, string fullName, string menuId, string userId)
        {
            return await repository.Context.Queryable<VisualPersonalEntity>()
                .Where(a => a.F_ID != id && a.FullName == fullName && a.IsDeleted == false && a.MenuId == menuId && a.CreateId == userId).CountAsync();

        }

        public async Task<bool> UpdateView(VisualPersonalEntity entity)
        {
            var str = await repository.Context.Updateable(entity).IgnoreColumns(it => new { it.CreateId, it.CreateName, it.CreateTime }).ExecuteCommandAsync() > 0;
            return str;
        }

        public async Task<bool> DelView(string id, string menuId)
        {
            return await repository.Context.Deleteable<VisualPersonalEntity>(a => a.F_ID == id && a.MenuId == menuId).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> SetDefault(string id, string menuId, string userId)
        {
            return await repository.Context.Updateable<VisualPersonalEntity>()
            .SetColumns(it => new VisualPersonalEntity
            {
                Status = SqlSugar.SqlFunc.IIF(it.F_ID == id, 1, 0)
            })
            .Where(it => it.MenuId == menuId && it.CreateId == userId)
            .ExecuteCommandAsync() > 0;
        }
    }
}
