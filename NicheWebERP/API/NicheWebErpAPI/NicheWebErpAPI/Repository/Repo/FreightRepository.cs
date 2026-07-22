using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    public class FreightRepository : IFreightRepository
    {
        private readonly IEfCoreService<CompanyLocation> _locationService;
        private readonly IEfCoreService<FreightItem> _freightItemService;

        public FreightRepository(IEfCoreService<CompanyLocation> locationService, IEfCoreService<FreightItem> freightItemService)
        {
            _locationService = locationService;
            _freightItemService = freightItemService;
        }

        public Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId) =>
            _locationService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == locationId);

        public Task<List<FreightItem>> GetItemsAsync(Guid companyId, Guid freightCalculatorId) =>
            _freightItemService.Query()
                .Where(fi => fi.CompanyID == companyId && fi.FreightID == freightCalculatorId)
                .ToListAsync();
    }
}
