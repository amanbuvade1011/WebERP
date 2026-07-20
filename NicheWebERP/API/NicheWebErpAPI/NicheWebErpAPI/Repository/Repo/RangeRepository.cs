using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class RangeRepository : IRangeRepository
    {
        private readonly IEfCoreService<Models.Range> _rangeService;

        public RangeRepository(IEfCoreService<Models.Range> rangeService)
        {
            _rangeService = rangeService;
        }

        public Task<List<Models.Range>> GetAllAsync(Guid companyId) =>
            _rangeService.Query()
                .Where(r => r.CompanyID == companyId)
                .OrderBy(r => r.Sequence)
                .ToListAsync();
    }
}
