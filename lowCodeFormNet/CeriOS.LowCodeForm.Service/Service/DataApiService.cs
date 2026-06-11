using CeriOS.Core.Common.Helper;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using NetTaste;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class DataApiService : IDataApiService
    {
        private readonly IDataApiRepository _dataApiRepository;

        public DataApiService(IDataApiRepository dataApiRepository)
        {
            _dataApiRepository = dataApiRepository;
        }

        public async Task<ResponseDto> Delete(string id)
        {
            var obj = await _dataApiRepository.GetById(id);
            obj.IsDeleted = true;
            await _dataApiRepository.Update(obj);
            return new ResponseDto() { Message = "删除成功" };
        }

        public async Task<QueryResponseDto<DataApi>> GetById(string id)
        {
            var obj = await _dataApiRepository.GetById(id);

            return new QueryResponseDto<DataApi>() { Data=obj};
        }

        public async Task<QueryResponseDto<List<DataApi>>> GetList(int pageIndex, int pageSize, string? category, string? name, string? type)
        {
            var result = await _dataApiRepository.GetList(pageIndex, pageSize, category, name, type);
            var categoryIds = result.Data.Select(x => x.Category).Distinct().ToList();
            var categories = await _dataApiRepository.GetDictItems(categoryIds);
            foreach (var item in result.Data)
            {
                item.DataHandler = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(item.DataHandler ?? ""));
                item.ExVer = System.Text.Encoding.UTF8.GetString(Convert.FromBase64String(item.ExVer ?? ""));
                item.CategoryName = categories.FirstOrDefault(x => x.DictDetailId == item.Category)?.ItemName;
                if (item.Type == "0")
                    item.Type = "静态数据";
                else if (item.Type == "1")
                    item.Type = "SQL操作";
                else if (item.Type == "2")
                    item.Type = "API操作";
            }
            return result;
        }

        public async Task<ResponseDto> Save(DataApi dataApi)
        {
            var check = await _dataApiRepository.CheckCode(dataApi.DataApiId, dataApi.Code);
            if (check)
                return new ResponseDto() {  Success = false, Message = "编码重复" };
            // 保存前编码
            dataApi.DataHandler = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataApi.DataHandler ?? ""));
            dataApi.ExVer = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(dataApi.ExVer ?? ""));

            if (string.IsNullOrEmpty(dataApi.DataApiId))
            {
                dataApi.DataApiId = GuidHelper.BuildGuid();
                await _dataApiRepository.Add(dataApi);
            }
            else
            {
                await _dataApiRepository.Update(dataApi);
            }
            return new ResponseDto() { Message = "保存成功" };
        }
    }
}
