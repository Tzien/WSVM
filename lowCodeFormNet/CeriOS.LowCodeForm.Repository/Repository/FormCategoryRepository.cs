using CeriOS.LowCodeForm.Model.Model;
using CeriOS.Core.Common.DB;
using CeriOS.LowCodeForm.Repository.IRepository;
using SqlSugar;
using CeriOS.Core.Model.ViewModel;
namespace CeriOS.LowCodeForm.Repository.Repository
{
    public class FormCategoryRepository : IFormCategoryRepository
    {
        public Repository<FormCategory> _db { get; set; }

        public FormCategoryRepository()
        {
            _db = new Repository<FormCategory>();
        }

        public async Task<QueryResponseDto<List<FormCategory>>> GetFormCategoryAsync(int pageIndex, int pageSize)
        {
            QueryResponseDto<List<FormCategory>> queryResponseDto = new QueryResponseDto<List<FormCategory>>();
            var query = _db.Context.Queryable<FormCategory>().Where(o => !o.IsDeleted);

            queryResponseDto.Total = await query.CountAsync();
            queryResponseDto.Data = await query
                .ToPageListAsync(pageIndex, pageSize);
            queryResponseDto.OK(true, "查询成功");
            return queryResponseDto;
        }



        public async Task<bool> InsertFormCategoryAsync(FormCategory parameter)
        {
            return await _db.InsertAsync(parameter);
        }

        public async Task<bool> UpdateFormCategoryAsync(FormCategory parameter)
        {
            return await _db.UpdateAsync(parameter);
        }
        public async Task<bool> DeleteFormCategoryAsync(string id)
        {
            int num = 0;
            num = await _db.Context.Updateable<FormCategory>()
.Where(o => o.FormCategoryId == id)
.SetColumns(o => new FormCategory { IsDeleted = true })
.ExecuteCommandAsync();
            return num > 0;
        }


        // 根据ID查询
        public async Task<FormCategory> GetFormCategoryByIdAsync(string id)
        {
            return await _db.GetByIdAsync(id);
        }

        public async Task<List<FormCategory>> GetFormCategoriesByIds(List<string> ids)
        {
            return await _db.GetListAsync(x => ids.Contains(x.FormCategoryId));
        }
    }
}