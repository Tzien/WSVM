using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class VisualDevReleaseEntityService : IVisualDevReleaseEntityService
    {
        private readonly IVisualDevReleaseEntityRepository _visualDevReleaseEntityRepository;
        public VisualDevReleaseEntityService(IVisualDevReleaseEntityRepository visualDevReleaseEntityRepository)
        {
            _visualDevReleaseEntityRepository = visualDevReleaseEntityRepository;
        }

        public async Task<List<VisualAliasEntity>> GetAliasList(string id)
        {
            return await _visualDevReleaseEntityRepository.GetAliasList(id);
        }

        public async Task<DictionaryDataEntity> GetDictionaryDataFirst(string module)
        {
          return await _visualDevReleaseEntityRepository.GetDictionaryDataFirst(module);
        }

        public async Task<VisualDevReleaseEntity> GetFirstAsync(string id)
        {
            return await _visualDevReleaseEntityRepository.GetFirstAsync(id);
        }
    }
}
