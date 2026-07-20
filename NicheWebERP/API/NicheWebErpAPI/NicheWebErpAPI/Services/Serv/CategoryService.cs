using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    // Builds the tree from ParentCategoryID (adjacency list). Category's LeftTag/RightTag
    // columns are present in the schema but confirmed dead (0/0 on every one of the 251 real
    // rows) - see docs/ai-plan/01-database-map.md. Do not add nested-set logic here.
    public class CategoryService : ICategoryService
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ICurrentUserService _currentUserService;

        public CategoryService(ICategoryRepository categoryRepository, ICurrentUserService currentUserService)
        {
            _categoryRepository = categoryRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<CategoryNodeDto>> GetTreeAsync()
        {
            var categories = await _categoryRepository.GetAllAsync(_currentUserService.CompanyId);

            var nodesById = categories.ToDictionary(
                c => c.EntityID,
                c => new CategoryNodeDto { Id = c.EntityID, Description = c.Description, Inactive = c.Inactive });

            var roots = new List<CategoryNodeDto>();
            foreach (var c in categories)
            {
                var node = nodesById[c.EntityID];
                if (c.ParentCategoryID.HasValue && nodesById.TryGetValue(c.ParentCategoryID.Value, out var parent))
                {
                    parent.Children.Add(node);
                }
                else
                {
                    roots.Add(node);
                }
            }

            return roots;
        }

        public async Task<CategoryNodeDto> CreateAsync(CreateCategoryDto dto)
        {
            var companyId = _currentUserService.CompanyId;

            var depth = 0;
            if (dto.ParentId.HasValue)
            {
                var parent = await _categoryRepository.GetByIdAsync(companyId, dto.ParentId.Value)
                    ?? throw new KeyNotFoundException($"Parent category {dto.ParentId} not found.");
                depth = parent.Depth + 1;
            }

            var category = new Category
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                Description = dto.Description,
                ParentCategoryID = dto.ParentId,
                Depth = depth,
                // Nested-set columns intentionally left at 0 - matches every existing row; this
                // table's real hierarchy is ParentCategoryID, not LeftTag/RightTag.
                LeftTag = 0,
                RightTag = 0,
                Inactive = false,
                Sequence = 0,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty
            };

            await _categoryRepository.AddAsync(category);
            await _categoryRepository.SaveChangesAsync();

            return new CategoryNodeDto { Id = category.EntityID, Description = category.Description, Inactive = false };
        }
    }
}
