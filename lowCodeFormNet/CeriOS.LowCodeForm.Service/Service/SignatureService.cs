using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model2;
using CeriOS.LowCodeForm.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class SignatureService : ISignatureService
    {
        private readonly ISignatureRepository _signatureRepository;

        public SignatureService(ISignatureRepository signatureRepository)
        {
            _signatureRepository = signatureRepository;
        }

        public async Task<ResponseDto> Delete(string id)
        {
            var obj=await _signatureRepository.GetById(id);
            obj.IsDeleted = true;
            await _signatureRepository.Update(obj);
            return new ResponseDto();
        }

        public async Task<QueryResponseDto<List<Signature>>> GetAll(int page, int size, string? name, string? userId)
        {
            var data = await _signatureRepository.GetAll(page, size, name, userId);
            foreach (var item in data.Data)
            {
                var ll = await DBContextHelper.Instance()
               .Context.GetConnection("CeriOS")
               .Queryable<User>()
               .Where(x => item.UserIds.Contains(x.Id)).Select(x => x.RealName).ToListAsync();
                item.UserIds = string.Join(',',ll);
            }
            return data;
        }

        public async Task<QueryByIdResponseDto<Signature>> GetById(string id)
        {
            var obj=await _signatureRepository.GetById(id);
            return new QueryByIdResponseDto<Signature>() { Data = obj };
        }

        public async Task<ResponseDto> Save(Signature signature)
        {
            var b = await _signatureRepository.CheckCode(signature);
            if(b)
                return new ResponseDto().OK(true,"编码已存在");
            if (string.IsNullOrEmpty(signature.SignatureId))
            {
                signature.SignatureId = Guid.NewGuid().ToString();
                await _signatureRepository.Add(signature);
            }
            else
            {
                await _signatureRepository.Update(signature);
            }
            return new ResponseDto();
        }
    }
}
