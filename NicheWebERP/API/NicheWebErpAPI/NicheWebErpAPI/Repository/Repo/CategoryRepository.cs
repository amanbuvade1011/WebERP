using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly IEfCoreService<Category> _categoryService;

        public CategoryRepository(IEfCoreService<Category> categoryService)
        {
            _categoryService = categoryService;
        }

        public Task<List<Category>> GetAllAsync(Guid companyId) =>
            _categoryService.Query()
                .Where(c => c.CompanyID == companyId)
                .OrderBy(c => c.LeftTag)
                .ToListAsync();

        public Task<Category?> GetByIdAsync(Guid companyId, Guid id) =>
            _categoryService.Query().FirstOrDefaultAsync(c => c.CompanyID == companyId && c.EntityID == id);

        public Task AddAsync(Category category) => _categoryService.AddAsync(category);

        public void Update(Category category) => _categoryService.Update(category);

        public Task<int> SaveChangesAsync() => _categoryService.SaveChangesAsync();
    }
}
