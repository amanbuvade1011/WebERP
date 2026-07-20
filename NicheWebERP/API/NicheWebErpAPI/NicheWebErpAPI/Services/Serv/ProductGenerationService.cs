using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class ProductGenerationService : IProductGenerationService
    {
        private readonly IProductGenerationRepository _repository;
        private readonly ISizewayRepository _sizewayRepository;
        private readonly IStylePricingRepository _pricingRepository;
        private readonly ICurrentUserService _currentUserService;

        public ProductGenerationService(
            IProductGenerationRepository repository,
            ISizewayRepository sizewayRepository,
            IStylePricingRepository pricingRepository,
            ICurrentUserService currentUserService)
        {
            _repository = repository;
            _sizewayRepository = sizewayRepository;
            _pricingRepository = pricingRepository;
            _currentUserService = currentUserService;
        }

        // Idempotent: only creates the Product/ProductLocation rows that don't already exist.
        // Safe to call again after a Style's sell-locations change, to backfill new locations.
        public async Task<GenerateProductsResultDto> GenerateProductsAsync(Guid styleId, Guid styleColorId)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var style = await _repository.GetStyleAsync(companyId, styleId)
                ?? throw new KeyNotFoundException($"Style {styleId} not found.");
            if (await _repository.GetStyleColorAsync(companyId, styleColorId) is null)
            {
                throw new KeyNotFoundException($"Style color {styleColorId} not found.");
            }

            var sizewayItems = await _sizewayRepository.GetItemsAsync(companyId, style.SizewayID);
            var existingProducts = await _repository.GetProductsForStyleColorAsync(companyId, styleId, styleColorId);
            var existingBySizewayItem = existingProducts.ToDictionary(p => p.SizewayItemID);

            var productsCreated = 0;
            var newProducts = new List<Product>();

            foreach (var item in sizewayItems)
            {
                if (existingBySizewayItem.ContainsKey(item.EntityID))
                {
                    continue;
                }

                var product = new Product
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    StyleID = styleId,
                    StyleColorID = styleColorId,
                    SizewayItemID = item.EntityID,
                    Inactive = false,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedByID = updatedById
                };
                await _repository.AddProductAsync(product);
                newProducts.Add(product);
                productsCreated++;
            }

            if (newProducts.Count > 0)
            {
                await _repository.SaveChangesAsync();
            }

            var allProducts = existingProducts.Concat(newProducts).ToList();
            var sellLocations = await _pricingRepository.GetSellLocationsAsync(companyId, styleId);
            var existingProductLocations = await _repository.GetProductLocationsForStyleColorAsync(
                companyId, styleId, styleColorId);
            var existingKeys = existingProductLocations
                .Select(pl => (pl.ProductID, pl.StyleSellLocationID))
                .ToHashSet();

            var productLocationsCreated = 0;
            foreach (var product in allProducts)
            {
                foreach (var sellLocation in sellLocations)
                {
                    if (existingKeys.Contains((product.EntityID, sellLocation.EntityID)))
                    {
                        continue;
                    }

                    await _repository.AddProductLocationAsync(new ProductLocation
                    {
                        CompanyID = companyId,
                        EntityID = Guid.NewGuid(),
                        ProductID = product.EntityID,
                        StyleSellLocationID = sellLocation.EntityID,
                        Held = 0,
                        Allocated = 0,
                        TransitIn = 0,
                        RequiredOut = 0,
                        TransitOut = 0,
                        PurchaseOrder = 0,
                        RentalHeld = 0,
                        RentalOrder = 0,
                        RentalOut = 0,
                        LastUpdated = DateTime.UtcNow,
                        UpdatedByID = updatedById
                    });
                    productLocationsCreated++;
                }
            }

            if (productLocationsCreated > 0)
            {
                await _repository.SaveChangesAsync();
            }

            return new GenerateProductsResultDto
            {
                ProductsCreated = productsCreated,
                ProductLocationsCreated = productLocationsCreated,
                TotalProductsForColor = allProducts.Count
            };
        }

        public async Task<ProductStockDto> GetProductStockAsync(Guid productId) =>
            await _repository.GetProductStockAsync(_currentUserService.CompanyId, productId)
                ?? throw new KeyNotFoundException($"Product {productId} not found.");

        public Task<List<StockGridRowDto>> GetStockByStyleColorAsync(Guid styleId, Guid styleColorId) =>
            _repository.GetStockGridAsync(_currentUserService.CompanyId, styleId, styleColorId);
    }
}
