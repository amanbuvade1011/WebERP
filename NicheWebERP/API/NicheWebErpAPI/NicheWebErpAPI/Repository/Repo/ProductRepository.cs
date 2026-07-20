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

        // Historically named "GetAllProducts" but is really a Style-level listing (joined with
        // Category/Label) - kept as-is, just paged + filtered now. Sprint 03 note: true
        // SKU-grain product data lives in the Product/ProductLocation tables (see
        // GetProductStock/GetStockByStyleColor in IProductGenerationRepository) - this endpoint
        // was not rewritten to that grain, only extended, to stay in scope for this sprint.
        public async Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(
            Guid companyId, int page, int pageSize, string? search, Guid? categoryId, Guid? labelId)
        {
            var query =
                from s in _styleService.Query()
                where s.CompanyID == companyId
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
                select new { Style = s, Category = c, Label = l };

            if (!string.IsNullOrWhiteSpace(search))
            {
                var term = search.Trim();
                query = query.Where(x => x.Style.Code.Contains(term) || x.Style.Description.Contains(term));
            }
            if (categoryId.HasValue)
            {
                query = query.Where(x => x.Style.CategoryID == categoryId.Value);
            }
            if (labelId.HasValue)
            {
                query = query.Where(x => x.Style.LabelID == labelId.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Style.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new ProductListItemDto
                {
                    StyleCode = x.Style.Code,
                    Garment = x.Style.Description,
                    Category = x.Category != null ? x.Category.Description : null,
                    Label = x.Label != null ? x.Label.Description : null
                })
                .ToListAsync();

            return new PagedResultDto<ProductListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }
    }
}
