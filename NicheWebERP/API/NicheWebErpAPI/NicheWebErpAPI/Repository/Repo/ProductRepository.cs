using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI; // IEfCoreService<T> lives in the flat root namespace

namespace NicheWebErpAPI.Repository.Repo
{
    public class ProductRepository : IProductRepository
    {
        private readonly IEfCoreService<Style> _styleService;
        private readonly IEfCoreService<Category> _categoryService;
        private readonly IEfCoreService<Label> _labelService;

        public ProductRepository(
            IEfCoreService<Style> styleService,
            IEfCoreService<Category> categoryService,
            IEfCoreService<Label> labelService)
        {
            _styleService = styleService;
            _categoryService = categoryService;
            _labelService = labelService;
        }

        public async Task<IEnumerable<ProductListItemDto>> GetAllProductsAsync()
        {
            var query =
                from s in _styleService.Query()
                join c in _categoryService.Query()
                    on new { s.CompanyID, CategoryID = s.CategoryID }
                    equals new { c.CompanyID, CategoryID = (Guid?)c.EntityID }
                    into categoryJoin
                from c in categoryJoin.DefaultIfEmpty()
                join l in _labelService.Query()
                    on new { s.CompanyID, LabelID = s.LabelID }
                    equals new { l.CompanyID, LabelID = (Guid?)l.EntityID }
                    into labelJoin
                from l in labelJoin.DefaultIfEmpty()
                select new ProductListItemDto
                {
                    StyleCode = s.Code,
                    Garment = s.Description,
                    Category = c != null ? c.Description : null,
                    Label = l != null ? l.Description : null
                };

            return await query.ToListAsync();
        }
    }
}
