using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Model.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IDataApiRepository
    {

        Task<QueryResponseDto<List<DataApi>>> GetList(int pageIndex, int pageSize,string? category,string? name, string? type);

        Task<bool> Add(DataApi parameter);
        Task<DataApi> GetById(string id);
        Task<bool> Update(DataApi parameter);
        Task<bool> CheckCode(string id, string code);
        Task<List<DictItemDto>> GetDictItems(List<string> ids);
    }
}
