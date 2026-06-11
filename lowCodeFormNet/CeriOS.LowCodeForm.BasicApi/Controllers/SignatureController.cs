using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model2;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Service.Service;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace CeriOS.LowCodeForm.BasicApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SignatureController : ControllerBase
    {
        private readonly ISignatureService _signatureService;

        public SignatureController(ISignatureService signatureService)
        {
            _signatureService = signatureService;
        }


        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("Save")]
        public async Task<ResponseDto> Save(Signature data)
        {
            return await _signatureService.Save(data);
        }


        /// <summary>
        /// 删除
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("Delete")]
        public async Task<ResponseDto> Delete(string id)
        {
            return await _signatureService.Delete(id);
        }



        /// <summary>
        /// 详情
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetById")]
        public async Task<QueryByIdResponseDto<Signature>> GetById(string id)
        {
            return await _signatureService.GetById(id);
        }


        /// <summary>
        /// 列表
        /// </summary>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <param name="name">名称</param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("GetList")]
        public async Task<QueryResponseDto<List<Signature>>> GetList(int pageIndex, int pageSize, string? name, string? userId)
        {
            return await _signatureService.GetAll(pageIndex, pageSize, name, userId);
        }

    }
}
