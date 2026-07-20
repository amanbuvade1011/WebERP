using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class StylePricingRepository : IStylePricingRepository
    {
        private readonly IEfCoreService<PricePoint> _pricePointService;
        private readonly IEfCoreService<StylePrice> _stylePriceService;
        private readonly IEfCoreService<StyleSellLocation> _sellLocationService;

        public StylePricingRepository(
            IEfCoreService<PricePoint> pricePointService,
            IEfCoreService<StylePrice> stylePriceService,
            IEfCoreService<StyleSellLocation> sellLocationService)
        {
            _pricePointService = pricePointService;
            _stylePriceService = stylePriceService;
            _sellLocationService = sellLocationService;
        }

        public Task<List<PricePoint>> GetAllPricePointsAsync(Guid companyId) =>
            _pricePointService.Query().Where(p => p.CompanyID == companyId).OrderBy(p => p.Name).ToListAsync();

        public Task<bool> PricePointExistsAsync(Guid companyId, Guid pricePointId) =>
            _pricePointService.Query().AnyAsync(p => p.CompanyID == companyId && p.EntityID == pricePointId);

        public Task<List<StylePrice>> GetStylePricesAsync(Guid companyId, Guid styleId) =>
            _stylePriceService.Query().Where(p => p.CompanyID == companyId && p.StyleID == styleId).ToListAsync();

        public Task<StylePrice?> GetStylePriceAsync(Guid companyId, Guid styleId, Guid pricePointId) =>
            _stylePriceService.Query()
                .FirstOrDefaultAsync(p => p.CompanyID == companyId && p.StyleID == styleId && p.PricePointID == pricePointId);

        public Task AddStylePriceAsync(StylePrice price) => _stylePriceService.AddAsync(price);

        public void UpdateStylePrice(StylePrice price) => _stylePriceService.Update(price);

        public Task<List<StyleSellLocation>> GetSellLocationsAsync(Guid companyId, Guid styleId) =>
            _sellLocationService.Query().Where(l => l.CompanyID == companyId && l.StyleID == styleId).ToListAsync();

        public Task<StyleSellLocation?> GetSellLocationAsync(Guid companyId, Guid styleId, Guid locationId) =>
            _sellLocationService.Query()
                .FirstOrDefaultAsync(l => l.CompanyID == companyId && l.StyleID == styleId && l.LocationID == locationId);

        public Task AddSellLocationAsync(StyleSellLocation location) => _sellLocationService.AddAsync(location);

        public void UpdateSellLocation(StyleSellLocation location) => _sellLocationService.Update(location);

        public Task<int> SaveChangesAsync() => _stylePriceService.SaveChangesAsync();
    }
}
