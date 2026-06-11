using Azure;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model2;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface ISignatureRepository
    {
        Task<bool> Add(Signature signature);
        Task<bool> Update(Signature signature);
        Task<bool> CheckCode(Signature signature);
        Task<Signature> GetById(string id);
        Task<QueryResponseDto<List<Signature>>> GetAll(int page,int size,string? name,string? userId);
    }
}
