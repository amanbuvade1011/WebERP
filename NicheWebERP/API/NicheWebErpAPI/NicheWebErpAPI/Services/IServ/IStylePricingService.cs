using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IStylePricingService
    {
        Task<List<PricePointDto>> GetAllPricePointsAsync();
        Task<List<StylePriceDto>> GetPricesAsync(Guid styleId);
        Task<List<StylePriceDto>> UpdatePricesAsync(Guid styleId, UpdateStylePricesDto dto);
        Task<List<StyleSellLocationDto>> GetSellLocationsAsync(Guid styleId);
        Task<List<StyleSellLocationDto>> UpdateSellLocationsAsync(Guid styleId, UpdateSellLocationsDto dto);
    }
}
