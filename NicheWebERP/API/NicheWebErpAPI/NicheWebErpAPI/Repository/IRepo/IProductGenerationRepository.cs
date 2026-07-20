using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IProductGenerationRepository
    {
        Task<Style?> GetStyleAsync(Guid companyId, Guid styleId);
        Task<StyleColor?> GetStyleColorAsync(Guid companyId, Guid styleColorId);

        Task<List<Product>> GetProductsForStyleColorAsync(Guid companyId, Guid styleId, Guid styleColorId);
        Task AddProductAsync(Product product);

        // Joins through Product rather than accepting a List<Guid> of product IDs - this
        // database's compatibility level (100 / SQL Server 2008, confirmed live 2026-07-19,
        // despite the engine itself being SQL Server 2022) doesn't support OPENJSON, which is
        // what EF Core 9 uses to translate List<T>.Contains(...) into SQL. Avoid that pattern
        // anywhere in this codebase - see docs/ai-plan/01-database-map.md for the full finding.
        Task<List<ProductLocation>> GetProductLocationsForStyleColorAsync(Guid companyId, Guid styleId, Guid styleColorId);
        Task AddProductLocationAsync(ProductLocation productLocation);

        Task<int> SaveChangesAsync();

        Task<ProductStockDto?> GetProductStockAsync(Guid companyId, Guid productId);
        Task<List<StockGridRowDto>> GetStockGridAsync(Guid companyId, Guid styleId, Guid styleColorId);
    }
}
