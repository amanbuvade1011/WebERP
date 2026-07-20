using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class ErpRoleRepository : IErpRoleRepository
    {
        private readonly IEfCoreService<ErpRole> _erpRoleService;

        public ErpRoleRepository(IEfCoreService<ErpRole> erpRoleService)
        {
            _erpRoleService = erpRoleService;
        }

        public async Task<IEnumerable<ErpRole>> GetAllAsync() =>
            await _erpRoleService.Query().OrderBy(r => r.Name).ToListAsync();

        public Task<ErpRole?> GetByIdAsync(int id) => _erpRoleService.GetByIdAsync(id);

        public async Task<bool> ExistsAsync(int id) =>
            await _erpRoleService.Query().AnyAsync(r => r.Id == id);
    }
}
