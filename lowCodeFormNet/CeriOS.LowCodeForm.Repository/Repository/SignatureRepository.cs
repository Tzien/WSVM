using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model2;
using CeriOS.LowCodeForm.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class SignatureRepository : ISignatureRepository
    {
        public Repository<Signature> _db { get; set; }

        public SignatureRepository()
        {
            _db = new Repository<Signature>();
        }
        public async Task<bool> Add(Signature signature)
        {
            return await _db.InsertAsync(signature);
        }

        public async Task<QueryResponseDto<List<Signature>>> GetAll(int page, int size, string? name, string? userId)
        {
            var query = _db.Context.Queryable<Signature>().Where(x => !x.IsDeleted);
            if (!string.IsNullOrEmpty(name))
                query = query.Where(x => x.Name.Contains(name));
            if (!string.IsNullOrEmpty(userId))
                query = query.Where(x => x.UserIds.Contains(userId));
            int count = await query.CountAsync();
            var data = await query.ToListAsync();
            return new QueryResponseDto<List<Signature>>() { Total=count,Data=data};
        }

        public async Task<Signature> GetById(string id)
        {
            return await _db.GetByIdAsync(id);
        }

        public async Task<bool> Update(Signature signature)
        {
            return await _db.UpdateAsync(signature);
        }

        public async Task<bool> CheckCode(Signature signature)
        {
            return await _db.IsAnyAsync(x=>x.SignatureId!=signature.SignatureId && x.Code==signature.Code && !x.IsDeleted);
        }
    }
}
