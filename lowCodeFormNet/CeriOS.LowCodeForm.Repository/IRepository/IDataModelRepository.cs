using CeriOS.LowCodeForm.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IDataModelRepository
    {
        Task<DBConfig> GetDB(string linkId);
        Task<List<DBConfig>> GetDBXia();
    }
}
