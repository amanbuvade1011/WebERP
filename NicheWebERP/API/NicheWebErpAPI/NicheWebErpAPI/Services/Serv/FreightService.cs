using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class FreightService : IFreightService
    {
        private readonly IFreightRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public FreightService(IFreightRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        // No calculator configured, or no matching band, returns 0 (free) rather than erroring -
        // a location without freight setup shouldn't block order creation. See
        // docs/ai-plan/sprints/sprint-07-promotions-freight.md for the RangeStart-as-item-count
        // interpretation (inferred from the live "FreightPerItem" calculator's own description
        // and banding shape, not confirmed via a lookup table).
        public async Task<decimal> CalculateAsync(Guid locationId, int quantity, Guid? countryId)
        {
            var companyId = _currentUserService.CompanyId;

            var location = await _repository.GetLocationAsync(companyId, locationId);
            if (location is null || location.FreightID == Guid.Empty)
            {
                return 0;
            }

            var items = await _repository.GetItemsAsync(companyId, location.FreightID);
            if (items.Count == 0)
            {
                return 0;
            }

            var candidates = countryId.HasValue
                ? items.Where(i => i.CountryID == countryId.Value).ToList()
                : new List<Models.FreightItem>();
            if (candidates.Count == 0)
            {
                candidates = items.Where(i => i.CountryID == Guid.Empty).ToList();
            }

            var band = candidates
                .Where(i => (i.RangeStart ?? 0) <= quantity)
                .OrderByDescending(i => i.RangeStart ?? 0)
                .FirstOrDefault();

            return band?.Price ?? 0;
        }
    }
}
