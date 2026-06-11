using CeriOS.Core.Model.Model;
using CeriOS.Core.Model.ViewModel.DataDict;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class VisualDictionaryDataService : IVisualDictionaryDataService
    {
        private readonly IVisualDictionaryDataRepository _visualDictionaryDataRepository;
        public VisualDictionaryDataService(IVisualDictionaryDataRepository visualDictionaryDataRepository)
        {
            _visualDictionaryDataRepository = visualDictionaryDataRepository;
        }
        public async Task<List<DictBaseInfo>> GetAllDictData()
        {
            return await _visualDictionaryDataRepository.GetAllDictData();

        }

        public async Task<List<DictDetail>> GetAllDictDetail()
        {
            return await _visualDictionaryDataRepository.GetAllDictDetail();
        }

        public Task<List<DictType>> GetTypeList()
        {
            throw new NotImplementedException();
        }
    }
}
