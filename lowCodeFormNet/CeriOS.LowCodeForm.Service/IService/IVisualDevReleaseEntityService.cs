using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.IService
{
    public interface IVisualDevReleaseEntityService
    {
        Task<List<VisualAliasEntity>> GetAliasList(string id);
        Task<DictionaryDataEntity> GetDictionaryDataFirst(string module);
        Task<VisualDevReleaseEntity> GetFirstAsync(string id);
    }
}
