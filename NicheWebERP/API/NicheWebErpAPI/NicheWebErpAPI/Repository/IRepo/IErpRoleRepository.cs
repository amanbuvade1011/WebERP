using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IErpRoleRepository
    {
        Task<IEnumerable<ErpRole>> GetAllAsync();
        Task<ErpRole?> GetByIdAsync(int id);
        Task<bool> ExistsAsync(int id);
    }
}
