using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI; // IEfCoreService<T> lives in the flat root namespace

namespace NicheWebErpAPI.Repository.Repo
{
    public class ErpUserRepository : IErpUserRepository
    {
        private readonly IEfCoreService<ErpUser> _erpUserService;

        public ErpUserRepository(IEfCoreService<ErpUser> erpUserService)
        {
            _erpUserService = erpUserService;
        }

        public async Task<IEnumerable<ErpUser>> GetAllAsync() =>
            await _erpUserService.Query().Include(u => u.Role).OrderBy(u => u.Username).ToListAsync();

        public async Task<ErpUser?> GetByIdAsync(int id) =>
            await _erpUserService.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<ErpUser?> GetByUsernameAsync(string username) =>
            await _erpUserService.Query().Include(u => u.Role).FirstOrDefaultAsync(u => u.Username == username);

        public async Task<bool> UsernameExistsAsync(string username) =>
            await _erpUserService.Query().AnyAsync(u => u.Username == username);

        public async Task<bool> EmailExistsAsync(string email) =>
            await _erpUserService.Query().AnyAsync(u => u.Email == email);

        public Task AddAsync(ErpUser user) => _erpUserService.AddAsync(user);

        public void Update(ErpUser user) => _erpUserService.Update(user);

        public Task<int> SaveChangesAsync() => _erpUserService.SaveChangesAsync();
    }
}
