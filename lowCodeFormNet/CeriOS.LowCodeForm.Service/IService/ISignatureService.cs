using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.IService
{
    public interface ISignatureService
    {
        Task<ResponseDto> Save(Signature signature);
        Task<ResponseDto> Delete(string id);
        Task<QueryByIdResponseDto<Signature>> GetById(string id);
        Task<QueryResponseDto<List<Signature>>> GetAll(int page,int size,string? name,string? userId);
    }
}
