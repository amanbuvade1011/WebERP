using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IProductGenerationService
    {
        Task<GenerateProductsResultDto> GenerateProductsAsync(Guid styleId, Guid styleColorId);
        Task<ProductStockDto> GetProductStockAsync(Guid productId);
        Task<List<StockGridRowDto>> GetStockByStyleColorAsync(Guid styleId, Guid styleColorId);
    }
}
