using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetAllAsync(Guid companyId);
        Task<Category?> GetByIdAsync(Guid companyId, Guid id);
        Task AddAsync(Category category);
        void Update(Category category);
        Task<int> SaveChangesAsync();
    }
}
