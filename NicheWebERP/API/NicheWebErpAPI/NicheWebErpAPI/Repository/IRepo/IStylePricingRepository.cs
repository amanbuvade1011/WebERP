using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IStylePricingRepository
    {
        Task<List<PricePoint>> GetAllPricePointsAsync(Guid companyId);
        Task<bool> PricePointExistsAsync(Guid companyId, Guid pricePointId);

        Task<List<StylePrice>> GetStylePricesAsync(Guid companyId, Guid styleId);
        Task<StylePrice?> GetStylePriceAsync(Guid companyId, Guid styleId, Guid pricePointId);
        Task AddStylePriceAsync(StylePrice price);
        void UpdateStylePrice(StylePrice price);

        Task<List<StyleSellLocation>> GetSellLocationsAsync(Guid companyId, Guid styleId);
        Task<StyleSellLocation?> GetSellLocationAsync(Guid companyId, Guid styleId, Guid locationId);
        Task AddSellLocationAsync(StyleSellLocation location);
        void UpdateSellLocation(StyleSellLocation location);

        Task<int> SaveChangesAsync();
    }
}
