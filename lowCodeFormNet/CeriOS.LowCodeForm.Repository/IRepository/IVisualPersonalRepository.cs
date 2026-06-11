using CeriOS.LowCodeForm.Model.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Repository.IRepository
{
    public interface IVisualPersonalRepository
    {
        Task<int> AddView(VisualPersonalEntity entity);
        Task<bool> DelView(string id, string menuId);
        Task<int> GetByIdSth(string id, string fullName, string menuId, string userId);
        Task<List<VisualPersonalEntity>> GetListBySth(string menuId, string userId);
        Task<bool> SetDefault(string id, string menuId, string userId);
        Task<bool> UpdateView(VisualPersonalEntity entity);
    }
}
