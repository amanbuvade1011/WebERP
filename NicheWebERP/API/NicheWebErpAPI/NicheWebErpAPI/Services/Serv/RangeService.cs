using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    // Same adjacency-list approach as CategoryService - check ParentRangeID-vs-LeftTag/RightTag
    // reliability before trusting the nested-set columns here too (see 01-database-map.md).
    public class RangeService : IRangeService
    {
        private readonly IRangeRepository _rangeRepository;
        private readonly ICurrentUserService _currentUserService;

        public RangeService(IRangeRepository rangeRepository, ICurrentUserService currentUserService)
        {
            _rangeRepository = rangeRepository;
            _currentUserService = currentUserService;
        }

        public async Task<List<RangeNodeDto>> GetTreeAsync()
        {
            var ranges = await _rangeRepository.GetAllAsync(_currentUserService.CompanyId);

            var nodesById = ranges.ToDictionary(
                r => r.EntityID,
                r => new RangeNodeDto { Id = r.EntityID, Description = r.Description });

            var roots = new List<RangeNodeDto>();
            foreach (var r in ranges)
            {
                var node = nodesById[r.EntityID];
                if (r.ParentRangeID.HasValue && nodesById.TryGetValue(r.ParentRangeID.Value, out var parent))
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
    }
}
