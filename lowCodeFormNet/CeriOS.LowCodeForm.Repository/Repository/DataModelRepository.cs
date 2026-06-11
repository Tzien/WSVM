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
    public class DataModelRepository: IDataModelRepository
    {
        public async Task<DBConfig> GetDB(string linkId)
        {
           return await DBContextHelper.Instance().Context.Queryable<DBConfig>().Where(p => p.DBConfigId == linkId && !p.IsDeleted).SingleAsync();
           
        }

        public async Task<List<DBConfig>> GetDBXia()
        {
            return await DBContextHelper.Instance().Context.Queryable<DBConfig>().Where(p => !p.IsDeleted).ToListAsync();

        }
    }
}
