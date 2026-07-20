using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class StyleRepository : IStyleRepository
    {
        private readonly IEfCoreService<Style> _styleService;
        private readonly IEfCoreService<StyleColor> _styleColorService;
        private readonly IEfCoreService<Category> _categoryService;
        private readonly IEfCoreService<Label> _labelService;
        private readonly IEfCoreService<Models.Range> _rangeService;
        private readonly IEfCoreService<Sizeway> _sizewayService;

        public StyleRepository(
            IEfCoreService<Style> styleService,
            IEfCoreService<StyleColor> styleColorService,
            IEfCoreService<Category> categoryService,
            IEfCoreService<Label> labelService,
            IEfCoreService<Models.Range> rangeService,
            IEfCoreService<Sizeway> sizewayService)
        {
            _styleService = styleService;
            _styleColorService = styleColorService;
            _categoryService = categoryService;
            _labelService = labelService;
            _rangeService = rangeService;
            _sizewayService = sizewayService;
        }

        public async Task<PagedResultDto<StyleListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search,
            Guid? categoryId, Guid? labelId, Guid? rangeId, bool? inactive)
        {
            var query =
                from s in _styleService.Query()
                where s.CompanyID == companyId
                join c in _categoryService.Query() on new { s.CompanyID, CategoryID = s.CategoryID }
                    equals new { c.CompanyID, CategoryID = (Guid?)c.EntityID } into categoryJoin
                from c in categoryJoin.DefaultIfEmpty()
                join l in _labelService.Query() on new { s.CompanyID, LabelID = s.LabelID }
                    equals new { l.CompanyID, LabelID = (Guid?)l.EntityID } into labelJoin
                from l in labelJoin.DefaultIfEmpty()
                join r in _rangeService.Query() on new { s.CompanyID, RangeID = s.RangeID }
                    equals new { r.CompanyID, RangeID = r.EntityID } into rangeJoin
                from r in rangeJoin.DefaultIfEmpty()
                select new { Style = s, CategoryName = c != null ? c.Description : null, LabelName = l != null ? l.Description : null, RangeName = r != null ? r.Description : null };

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
            if (rangeId.HasValue)
            {
                query = query.Where(x => x.Style.RangeID == rangeId.Value);
            }
            if (inactive.HasValue)
            {
                query = query.Where(x => x.Style.Inactive == inactive.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderBy(x => x.Style.Code)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new StyleListItemDto
                {
                    Id = x.Style.EntityID,
                    Code = x.Style.Code,
                    Description = x.Style.Description,
                    CategoryName = x.CategoryName,
                    LabelName = x.LabelName,
                    RangeName = x.RangeName,
                    Inactive = x.Style.Inactive
                })
                .ToListAsync();

            return new PagedResultDto<StyleListItemDto>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public Task<Style?> GetEntityByIdAsync(Guid companyId, Guid id) =>
            _styleService.Query().FirstOrDefaultAsync(s => s.CompanyID == companyId && s.EntityID == id);

        public async Task<StyleDetailDto?> GetDetailByIdAsync(Guid companyId, Guid id)
        {
            var style = await GetEntityByIdAsync(companyId, id);
            if (style is null)
            {
                return null;
            }

            var category = style.CategoryID.HasValue
                ? await _categoryService.Query().FirstOrDefaultAsync(c => c.CompanyID == companyId && c.EntityID == style.CategoryID.Value)
                : null;
            var label = style.LabelID.HasValue
                ? await _labelService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == style.LabelID.Value)
                : null;
            var range = await _rangeService.Query().FirstOrDefaultAsync(r => r.CompanyID == companyId && r.EntityID == style.RangeID);
            var sizeway = await _sizewayService.Query().FirstOrDefaultAsync(sw => sw.CompanyID == companyId && sw.EntityID == style.SizewayID);
            var colors = await GetColorsAsync(companyId, id);

            return new StyleDetailDto
            {
                Id = style.EntityID,
                Code = style.Code,
                Description = style.Description,
                WebDescription = style.WebDescription,
                Weight = style.Weight,
                SizewayId = style.SizewayID,
                SizewayDescription = sizeway?.Description,
                CategoryId = style.CategoryID,
                CategoryName = category?.Description,
                LabelId = style.LabelID,
                LabelName = label?.Description,
                RangeId = style.RangeID,
                RangeName = range?.Description,
                Inactive = style.Inactive,
                NonStock = style.NonStock,
                AllowManualPrice = style.AllowManualPrice,
                DeliveryPeriod = style.DeliveryPeriod,
                Colors = colors
            };
        }

        public Task AddAsync(Style style) => _styleService.AddAsync(style);

        public void Update(Style style) => _styleService.Update(style);

        public Task<int> SaveChangesAsync() => _styleService.SaveChangesAsync();

        public async Task<List<StyleColorDto>> GetColorsAsync(Guid companyId, Guid styleId) =>
            await _styleColorService.Query()
                .Where(c => c.CompanyID == companyId && c.StyleID == styleId)
                .OrderBy(c => c.Color)
                .Select(c => new StyleColorDto
                {
                    Id = c.EntityID,
                    Color = c.Color,
                    RgbValue = c.RgbValue,
                    Inactive = c.Inactive ?? false
                })
                .ToListAsync();

        public Task<bool> ColorExistsAsync(Guid companyId, Guid styleId, string color) =>
            _styleColorService.Query().AnyAsync(c => c.CompanyID == companyId && c.StyleID == styleId && c.Color == color);

        public Task AddColorAsync(StyleColor color) => _styleColorService.AddAsync(color);

        public Task<bool> SizewayExistsAsync(Guid companyId, Guid sizewayId) =>
            _sizewayService.Query().AnyAsync(sw => sw.CompanyID == companyId && sw.EntityID == sizewayId);

        public Task<bool> CategoryExistsAsync(Guid companyId, Guid categoryId) =>
            _categoryService.Query().AnyAsync(c => c.CompanyID == companyId && c.EntityID == categoryId);

        public Task<bool> LabelExistsAsync(Guid companyId, Guid labelId) =>
            _labelService.Query().AnyAsync(l => l.CompanyID == companyId && l.EntityID == labelId);

        public Task<bool> RangeExistsAsync(Guid companyId, Guid rangeId) =>
            _rangeService.Query().AnyAsync(r => r.CompanyID == companyId && r.EntityID == rangeId);
    }
}
