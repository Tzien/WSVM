using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.IService
{
    public interface IDataApiService
    {
        Task<ResponseDto> Save(DataApi dataApi);
        Task<ResponseDto> Delete(string id);
        Task<QueryResponseDto<DataApi>> GetById(string id);
        Task<QueryResponseDto<List<DataApi>>> GetList(int pageIndex, int pageSize, string? category, string? name, string? type);
    }
}
