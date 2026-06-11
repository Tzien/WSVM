using CeriOS.Core.Common.DB;
using CeriOS.Core.Common.Enum;
using CeriOS.Core.Common.Helper;
using CeriOS.Core.Model.Model;
using CeriOS.Core.Model.ViewModel.DataDict;
using CeriOS.LowCodeForm.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class VisualDictionaryDataRepository : IVisualDictionaryDataRepository
    {
        public Task<List<DictBaseInfo>> GetAllDictData()
        {
            return DBContextHelper.Instance()
                 .Context.GetConnection(DbTenant.Client(DbClientEnum.CeriOS)).Queryable<DictBaseInfo>().ToListAsync();

        }

        public Task<List<DictDetail>> GetAllDictDetail()
        {
            return DBContextHelper.Instance()
                .Context.GetConnection(DbTenant.Client(DbClientEnum.CeriOS)).Queryable<DictDetail>().ToListAsync();
        }
    }
}
