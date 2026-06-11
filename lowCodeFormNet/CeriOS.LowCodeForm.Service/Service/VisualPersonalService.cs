using CeriOS.LowCodeForm.Model.Model;
using CeriOS.LowCodeForm.Repository.IRepository;
using CeriOS.LowCodeForm.Service.IService;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CeriOS.LowCodeForm.Service.Service
{
    public class VisualPersonalService : IVisualPersonalService
    {
        public readonly IVisualPersonalRepository _VisualPersonalRepository;
        public VisualPersonalService(IVisualPersonalRepository visualPersonalRepository)
        {
            _VisualPersonalRepository = visualPersonalRepository;
        }

        public async Task<int> AddView(VisualPersonalEntity entity)
        {
            return await _VisualPersonalRepository.AddView(entity);
        }

        public async Task<bool> DelView(string id, string menuId)
        {
            return await _VisualPersonalRepository.DelView(id, menuId);
        }

        public async Task<int> GetByIdSth(string id, string fullName, string menuId, string userId)
        {
            return await _VisualPersonalRepository.GetByIdSth(id, fullName, menuId, userId);
        }

        public async Task<List<VisualPersonalEntity>> GetListBySth(string menuId, string userId)
        {
            return await _VisualPersonalRepository.GetListBySth(menuId, userId);
        }

        public async Task<bool> SetDefault(string id, string menuId, string userId)
        {
            return await _VisualPersonalRepository.SetDefault(id, menuId, userId);
        }

        public async Task<bool> UpdateView(VisualPersonalEntity entity)
        {
            return await _VisualPersonalRepository.UpdateView(entity);
        }
    }
}
