using CeriOS.Core.Model.Model;
using CeriOS.Core.Model.ViewModel.DataDict;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IVisualDictionaryDataRepository
    {
        Task<List<DictBaseInfo>> GetAllDictData();
        Task<List<DictDetail>> GetAllDictDetail();
    }
}
