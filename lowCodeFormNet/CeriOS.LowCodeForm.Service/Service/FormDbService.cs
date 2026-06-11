using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Service.IService;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using System.Drawing.Printing;
using Azure;
namespace CeriOS.LowCodeForm.Service.Service
{

    public class FormDbService : IFormDbService
    {
        public IFormDbRepository _repository { get; set; }

        public FormDbService(IFormDbRepository repository)
        {
            _repository = repository;
        }


        public async Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync(int pageIndex, int pageSize)
        {
            return await _repository.GetFormDbAsync(pageIndex, pageSize);
        }
        public async Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync2(int pageIndex, int pageSize)
        {
            var result = await _repository.GetFormDbAsync(pageIndex, pageSize);
            result.Data.ForEach(x =>
            {
                x.FullName = x.Name;
                x.enCode = x.Code;
            });
            return result;
        }

        public async Task<QueryByIdResponseDto<string>> InsertFormDbAsync(FormDbDto parameter)
        {
            try
            {
                var currentDateTime = DateTime.Now;
                var formDesign = new FormDesign();
                formDesign.FormDesignId = Guid.NewGuid().ToString();
                formDesign.Code = parameter.Code;
                formDesign.FormCategoryId = parameter.FormCategoryId;
                formDesign.DbId = parameter.DbId;
                formDesign.Name = parameter.Name;
                formDesign.FormJson = parameter.FormJson;
                formDesign.Remark = parameter.Remark;
                formDesign.Sort = parameter.Sort;
                formDesign.TableJson = parameter.TableJson;
                formDesign.Status = parameter.Status;
                formDesign.WebType = parameter.WebType;
                formDesign.ColumnDataStr = parameter.ColumnDataStr;
                formDesign.AppColumnDataStr = parameter.AppColumnDataStr;
                formDesign.ColumnData = parameter.ColumnData;
                formDesign.AppColumnData = parameter.AppColumnData;
                formDesign.CreateId = parameter.CreateId;
                formDesign.CreateTime = currentDateTime;
                formDesign.CreateName = parameter.CreateName;
                var st2 = await _repository.InsertFormDesignAsync(formDesign);

                var formdb = new List<FormDb>();
                foreach (var item in parameter.FormDbs)
                {
                    formdb.Add(new FormDb()
                    {
                        FormDbId = Guid.NewGuid().ToString(),
                        FormDesignId = formDesign.FormDesignId,
                        TableName = item.TableName,
                        TypeId = item.TypeId,
                        PK = item.PK,
                        FK = item.FK,
                        Fields = item.Fields,
                        CreateId = parameter.CreateId,
                        CreateTime = currentDateTime,
                        CreateName = parameter.CreateName,
                    });
                }
                var st = await _repository.InsertRange(formdb);

                return new QueryByIdResponseDto<string>()
                {
                    Code = 200,
                    Data = formDesign.FormDesignId,
                    Message = "添加成功",
                    Success = true
                };
            }
            catch (Exception ex)
            {
                return new QueryByIdResponseDto<string>()
                {
                    Code = 500,
                    Data = "",
                    Message = $"系统异常：{ex.Message}",
                    Success = false
                };
            }
        }

        public async Task<ResponseDto> UpdateFormDbAsync(FormDbDto parameter)
        {
            var mainTable = new FormDb();
            await _repository.DeleteFormDbAsync(parameter.FormDesignId);
            var formDesign = await _repository.GetFormDesignById(parameter.FormDesignId);
            formDesign.WebType = parameter.WebType;
            formDesign.Code = parameter.Code;
            formDesign.FormCategoryId = parameter.FormCategoryId;
            formDesign.DbId = parameter.DbId;
            formDesign.Name = parameter.Name;
            formDesign.FormJson = parameter.FormJson;
            formDesign.Remark = parameter.Remark;
            formDesign.Sort = parameter.Sort;
            formDesign.TableJson = parameter.TableJson;
            formDesign.Status = parameter.Status;
            formDesign.ColumnDataStr = parameter.ColumnDataStr;
            formDesign.AppColumnDataStr = parameter.AppColumnDataStr;
            formDesign.ColumnData = parameter.ColumnData;
            formDesign.AppColumnData = parameter.AppColumnData;
            await _repository.UpdateFormDesignAsync(formDesign);
            var formdb = new List<FormDb>();
            foreach (var item in parameter.FormDbs)
            {
                formdb.Add(new FormDb()
                {
                    FormDbId = Guid.NewGuid().ToString(),
                    FormDesignId = parameter.FormDesignId,
                    TableName = item.TableName,
                    TypeId = item.TypeId,
                    PK = item.PK,
                    FK = item.FK,
                    Fields = item.Fields
                });
            }
            await _repository.InsertRange(formdb);


            return new ResponseDto().OK(true, "修改成功");
        }


        public async Task<ResponseDto> DeleteFormDbAsync(string id)
        {
            var result = await _repository.DeleteFormDesign(id);
            return new ResponseDto().OK(result, result ? "删除成功" : "删除失败");
        }

        // 根据ID查询
        public async Task<QueryByIdResponseDto<FormDb>> GetFormDbByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<FormDb>();
            dto.Data = await _repository.GetFormDbByIdAsync(id);
            dto.OK(true, "查询成功");
            return dto;
        }
        // 根据tableName查询
        public async Task<QueryByIdResponseDto<FormDb>> GetFormDbByNameAsync(string name)
        {
            var dto = new QueryByIdResponseDto<FormDb>();
            dto.Data = await _repository.GetFormDbByNameAsync(name);
            dto.OK(true, "查询成功");
            return dto;
        }


        public async Task<QueryByIdResponseDto<FormDesign>> GetFormDesignByIdAsync(string id)
        {
            var dto = new QueryByIdResponseDto<FormDesign>();
            dto.Data = await _repository.GetFormDesignById(id);
            dto.OK(true, "查询成功");
            return dto;
        }

        public async Task<QueryResponseDto<FormDbDto>> GetFormDesignDtoByIdAsync(string id)
        {
            return await _repository.GetFormDesignDtoByIdAsync(id);
        }
    }

}