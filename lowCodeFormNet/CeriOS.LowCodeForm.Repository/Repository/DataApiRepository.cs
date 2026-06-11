using CeriOS.Core.Common.DB;
using CeriOS.Core.Common.Enum;
using CeriOS.Core.Common.Helper;
using CeriOS.Core.Model.ViewModel;
using CeriOS.Core.Model.ViewModel.Other;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class DataApiRepository : IDataApiRepository
    {
        public Repository<DataApi> _db { get; set; }

        public DataApiRepository()
        {
            _db = new Repository<DataApi>();
        }
        public async Task<bool> Add(DataApi parameter)
        {
            return await _db.InsertAsync(parameter);
        }

        public async Task<DataApi> GetById(string id)
        {
            var obj= await _db.GetByIdAsync(id);

            obj.DataHandler = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(obj.DataHandler ?? ""));
            obj.ExVer = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(obj.ExVer ?? ""));
            return obj;
        }

        public async Task<QueryResponseDto<List<DataApi>>> GetList(int pageIndex, int pageSize, string? category, string? name, string? type)
        {
            var queryResponseDto = new QueryResponseDto<List<DataApi>>();
            var query = _db.Context.Queryable<DataApi>().Where(o => !o.IsDeleted);
            if (!string.IsNullOrEmpty(category))
            {
                query = query.Where(x => x.Category == category);
            }
            if (!string.IsNullOrEmpty(name))
            {
                query = query.Where(x => x.Name.Contains(name));
            }
            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(x => x.Type == type);
            }

            queryResponseDto.Total = await query.CountAsync();
            queryResponseDto.Data = await query.Clone()
                .ToPageListAsync(pageIndex, pageSize);
            queryResponseDto.OK(true, "查询成功");
            return queryResponseDto;
        }

        public async Task<bool> Update(DataApi parameter)
        {
            return await _db.UpdateAsync(parameter);
        }

        public async Task<bool> CheckCode(string id, string code)
        {
            return await _db.IsAnyAsync(x => x.DataApiId != id && x.Code == code && !x.IsDeleted);
        }

        public async Task<List<DictItemDto>> GetDictItems(List<string> ids)
        {
            return await DBContextHelper.Instance()
                .Context.GetConnection("CeriOS")
                .Queryable<DictItemDto>()
                .Where(x => ids.Contains(x.DictDetailId)).ToListAsync();
        }
    }
}
