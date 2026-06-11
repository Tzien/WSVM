using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Common.DB;
using CeriOS.Core.Model.ViewModel;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Model.ViewModel;
using SqlSugar;
using System.Drawing.Printing;
namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class FormDbRepository : IFormDbRepository
    {
        public Repository<FormDb> _db { get; set; }
        public Repository<FormDesign> _formDesign { get; set; }

        public FormDbRepository(Repository<FormDesign> formDesign)
        {
            _db = new Repository<FormDb>();
            _formDesign = formDesign;
        }

        public async Task<QueryResponseDto<List<FormDbDto>>> GetFormDbAsync(int pageIndex, int pageSize)
        {
            
                QueryResponseDto<List<FormDbDto>> queryResponseDto = new QueryResponseDto<List<FormDbDto>>();
                var fromDesignQuery = _db.Context.Queryable<FormDesign>().Where(x => !x.IsDeleted);
                queryResponseDto.Total = await fromDesignQuery.CountAsync();
                List<FormDbDto> list = new List<FormDbDto>();
                var formDesigns = await fromDesignQuery.ToPageListAsync(pageIndex, pageSize);
                var formDesignIds = formDesigns.Select(x => x.FormDesignId).ToList();
                var formdbs = await _db.Context.Queryable<FormDb>().Where(x => !x.IsDeleted && formDesignIds.Contains(x.FormDesignId)).ToListAsync();
                foreach (var item in formDesigns)
                {
                    var fromdb = formdbs.Where(x => x.FormDesignId == item.FormDesignId).ToList();
                    list.Add(new FormDbDto()
                    {
                        FormDesignId = item.FormDesignId,
                        FormCategoryId = item.FormCategoryId,
                        Code = item.Code,
                        Name = item.Name,
                        DbId = item.DbId,
                        Remark = item.Remark,
                        WebType = item.WebType,
                        Sort = item.Sort,
                        FormJson = item.FormJson,
                        TableJson = item.TableJson,
                        Status = item.Status,
                        CreateId = item.CreateId,
                        CreateName = item.CreateName,
                        CreateTime = item.CreateTime,
                        ModifyId = item.ModifyId,
                        ModifyName = item.ModifyName,
                        ModifyTime = item.ModifyTime,
                        ColumnDataStr = item.ColumnDataStr,
                        AppColumnDataStr = item.AppColumnDataStr,
                        ColumnData = item.ColumnData,
                        AppColumnData = item.AppColumnData,
                        FormDbs = fromdb.OrderByDescending(x => x.TypeId).ToList()
                    });
                }
                queryResponseDto.Data = list;
                queryResponseDto.OK(true, "查询成功");
                return queryResponseDto;
        }

        public async Task<bool> InsertFormDbAsync(FormDb parameter)
        {
            return await _db.InsertAsync(parameter);
        }

        public async Task<bool> UpdateFormDbAsync(FormDb parameter)
        {
            return await _db.UpdateAsync(parameter);
        }
        public async Task<bool> InsertFormDesignAsync(FormDesign parameter)
        {
            return await _db.Context.Insertable(parameter).ExecuteCommandAsync() > 0;
        }

        public async Task<bool> UpdateFormDesignAsync(FormDesign parameter)
        {
            return await _db.Context.Updateable(parameter).ExecuteCommandAsync() > 0;
        }
        public async Task<bool> DeleteFormDbAsync(string formDesignId)
        {
            var obj = await _db.GetListAsync(x => x.FormDesignId == formDesignId);
            await _db.DeleteAsync(obj);
            return true;
        }


        // 根据ID查询
        public async Task<FormDb> GetFormDbByIdAsync(string id)
        {
            return await _db.GetByIdAsync(id);
        }

        // 根据名称查询
        public async Task<FormDb> GetFormDbByNameAsync(string name)
        {
            return await _db.GetFirstAsync(a=>a.TableName == name && a.IsDeleted == false);
        }

        public async Task<bool> InsertRange(List<FormDb> parameters)
        {
            return await _db.InsertRangeAsync(parameters);
        }

        public async Task<bool> DeleteFormDesign(string id)
        {
            var obj = await _formDesign.GetByIdAsync(id);
            obj.IsDeleted = true;
            await _formDesign.UpdateAsync(obj);
            return true;
        }

        public async Task<FormDesign> GetFormDesignById(string id)
        {
            return await _formDesign.GetByIdAsync(id);
        }

        public async Task<QueryResponseDto<FormDbDto>> GetFormDesignDtoByIdAsync(string id)
        {
            QueryResponseDto<FormDbDto> queryResponseDto = new QueryResponseDto<FormDbDto>();
            var fromDesignQuery = _db.Context.Queryable<FormDesign>().Where(x => !x.IsDeleted && x.FormDesignId == id);
            FormDbDto list = new FormDbDto();
            var item = await fromDesignQuery.FirstAsync();
            var formdbs = await _db.Context.Queryable<FormDb>().ToListAsync();
            var fromdb = formdbs.Where(x => x.FormDesignId == item.FormDesignId).ToList();
            list = new FormDbDto()
            {
                FormDesignId = item.FormDesignId,
                FormCategoryId = item.FormCategoryId,
                Code = item.Code,
                Name = item.Name,
                DbId = item.DbId,
                Remark = item.Remark,
                WebType = item.WebType,
                Sort = item.Sort,
                FormJson = item.FormJson,
                TableJson = item.TableJson,
                Status = item.Status,
                CreateId = item.CreateId,
                CreateName = item.CreateName,
                CreateTime = item.CreateTime,
                ModifyId = item.ModifyId,
                ModifyName = item.ModifyName,
                ModifyTime = item.ModifyTime,
                ColumnDataStr = item.ColumnDataStr,
                AppColumnDataStr = item.AppColumnDataStr,
                ColumnData = item.ColumnData,
                AppColumnData = item.AppColumnData,
                FormDbs = fromdb.OrderByDescending(x => x.TypeId).ToList()
            };
            queryResponseDto.Data = list;
            queryResponseDto.OK(true, "查询成功");
            return queryResponseDto;
        }
    }
}