using CeriOS.Core.Common.DB;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel.LowCode;
using CeriOS.LowCodeForm.Repository.IRepository;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class VisualDevReleaseEntityRepository : IVisualDevReleaseEntityRepository
    {
        public Repository<VisualDevReleaseEntity> repository { get; set; }
        public Repository<VisualAliasEntity> visualAliasEntityRepository { get; set; }
        public Repository<DictionaryDataEntity> dictionaryDataRepository { get; set; }
        public VisualDevReleaseEntityRepository(
            Repository<DictionaryDataEntity> dictionaryDataRepository,
            Repository<VisualAliasEntity> visualAliasEntityRepository,
            Repository<VisualDevReleaseEntity> repository
            )
        {
            this.dictionaryDataRepository = dictionaryDataRepository;
            this.visualAliasEntityRepository = visualAliasEntityRepository;
            this.repository = repository;
        }

        public async Task<VisualDevReleaseEntity> GetFirstAsync(string id)
        {
            return await repository.Context.Queryable<VisualDevReleaseEntity>()
             .Where(it => it.IsDeleted == false && it.Id == id).FirstAsync();
        }

        public async Task<List<VisualAliasEntity>> GetAliasList(string id)
        {
            return await visualAliasEntityRepository.Context.Queryable<VisualAliasEntity>()
             .Where(it => it.VisualId == id && it.IsDeleted == false).ToListAsync();
        }

        public async Task<DictionaryDataEntity> GetDictionaryDataFirst(string module)
        {
            return await dictionaryDataRepository.Context.Queryable<DictionaryDataEntity>().Where(it => it.Id.Equals(module)).FirstAsync();
        }
    }
}
