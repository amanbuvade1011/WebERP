using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IErpUserRepository
    {
        Task<IEnumerable<ErpUser>> GetAllAsync();
        Task<ErpUser?> GetByIdAsync(int id);
        Task<ErpUser?> GetByUsernameAsync(string username);
        Task<bool> UsernameExistsAsync(string username);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(ErpUser user);
        void Update(ErpUser user);
        Task<int> SaveChangesAsync();
    }
}
