using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class ProductGenerationRepository : IProductGenerationRepository
    {
        private readonly IEfCoreService<Style> _styleService;
        private readonly IEfCoreService<StyleColor> _styleColorService;
        private readonly IEfCoreService<Product> _productService;
        private readonly IEfCoreService<ProductLocation> _productLocationService;
        private readonly IEfCoreService<SizewayItem> _sizewayItemService;
        private readonly IEfCoreService<Size> _sizeService;
        private readonly IEfCoreService<StyleSellLocation> _sellLocationService;
        private readonly IEfCoreService<CompanyLocation> _companyLocationService;

        public ProductGenerationRepository(
            IEfCoreService<Style> styleService,
            IEfCoreService<StyleColor> styleColorService,
            IEfCoreService<Product> productService,
            IEfCoreService<ProductLocation> productLocationService,
            IEfCoreService<SizewayItem> sizewayItemService,
            IEfCoreService<Size> sizeService,
            IEfCoreService<StyleSellLocation> sellLocationService,
            IEfCoreService<CompanyLocation> companyLocationService)
        {
            _styleService = styleService;
            _styleColorService = styleColorService;
            _productService = productService;
            _productLocationService = productLocationService;
            _sizewayItemService = sizewayItemService;
            _sizeService = sizeService;
            _sellLocationService = sellLocationService;
            _companyLocationService = companyLocationService;
        }

        public Task<Style?> GetStyleAsync(Guid companyId, Guid styleId) =>
            _styleService.Query().FirstOrDefaultAsync(s => s.CompanyID == companyId && s.EntityID == styleId);

        public Task<StyleColor?> GetStyleColorAsync(Guid companyId, Guid styleColorId) =>
            _styleColorService.Query().FirstOrDefaultAsync(c => c.CompanyID == companyId && c.EntityID == styleColorId);

        public Task<List<Product>> GetProductsForStyleColorAsync(Guid companyId, Guid styleId, Guid styleColorId) =>
            _productService.Query()
                .Where(p => p.CompanyID == companyId && p.StyleID == styleId && p.StyleColorID == styleColorId)
                .ToListAsync();

        public Task AddProductAsync(Product product) => _productService.AddAsync(product);

        public Task<List<ProductLocation>> GetProductLocationsForStyleColorAsync(Guid companyId, Guid styleId, Guid styleColorId)
        {
            var query =
                from pl in _productLocationService.Query()
                join p in _productService.Query() on new { pl.CompanyID, ProductID = pl.ProductID }
                    equals new { p.CompanyID, ProductID = (Guid?)p.EntityID }
                where pl.CompanyID == companyId && p.StyleID == styleId && p.StyleColorID == styleColorId
                select pl;

            return query.ToListAsync();
        }

        public Task AddProductLocationAsync(ProductLocation productLocation) => _productLocationService.AddAsync(productLocation);

        public Task<int> SaveChangesAsync() => _productService.SaveChangesAsync();

        public async Task<ProductStockDto?> GetProductStockAsync(Guid companyId, Guid productId)
        {
            var header = await (
                from p in _productService.Query()
                where p.CompanyID == companyId && p.EntityID == productId
                join st in _styleService.Query() on new { p.CompanyID, StyleID = p.StyleID } equals new { st.CompanyID, StyleID = st.EntityID }
                join sc in _styleColorService.Query() on new { p.CompanyID, StyleColorID = p.StyleColorID } equals new { sc.CompanyID, StyleColorID = sc.EntityID }
                join si in _sizewayItemService.Query() on new { p.CompanyID, SizewayItemID = p.SizewayItemID } equals new { si.CompanyID, SizewayItemID = si.EntityID }
                join sz in _sizeService.Query() on new { si.CompanyID, si.SizeID } equals new { sz.CompanyID, SizeID = sz.EntityID }
                select new ProductStockDto
                {
                    ProductId = p.EntityID,
                    StyleCode = st.Code,
                    Color = sc.Color,
                    SizeDescription = sz.Description,
                    Barcode = p.Barcode
                }
            ).FirstOrDefaultAsync();

            if (header is null)
            {
                return null;
            }

            header.Locations = await GetLocationRowsAsync(companyId, productId);
            return header;
        }

        private async Task<List<ProductStockLocationDto>> GetLocationRowsAsync(Guid companyId, Guid productId)
        {
            var query =
                from pl in _productLocationService.Query()
                where pl.CompanyID == companyId && pl.ProductID == productId
                join sl in _sellLocationService.Query() on new { pl.CompanyID, StyleSellLocationID = pl.StyleSellLocationID }
                    equals new { sl.CompanyID, StyleSellLocationID = (Guid?)sl.EntityID } into sellLocJoin
                from sl in sellLocJoin.DefaultIfEmpty()
                join cl in _companyLocationService.Query() on new { CompanyID = pl.CompanyID, LocationID = sl != null ? sl.LocationID : Guid.Empty }
                    equals new { cl.CompanyID, LocationID = cl.EntityID } into locJoin
                from cl in locJoin.DefaultIfEmpty()
                select new ProductStockLocationDto
                {
                    LocationId = sl != null ? sl.LocationID : Guid.Empty,
                    LocationName = cl != null ? cl.Name : null,
                    Held = pl.Held,
                    Allocated = pl.Allocated,
                    Available = pl.AvailableFirm1
                };

            return await query.ToListAsync();
        }

        public async Task<List<StockGridRowDto>> GetStockGridAsync(Guid companyId, Guid styleId, Guid styleColorId)
        {
            var query =
                from p in _productService.Query()
                where p.CompanyID == companyId && p.StyleID == styleId && p.StyleColorID == styleColorId
                join si in _sizewayItemService.Query() on new { p.CompanyID, SizewayItemID = p.SizewayItemID } equals new { si.CompanyID, SizewayItemID = si.EntityID }
                join sz in _sizeService.Query() on new { si.CompanyID, si.SizeID } equals new { sz.CompanyID, SizeID = sz.EntityID }
                join pl in _productLocationService.Query() on new { p.CompanyID, ProductID = (Guid?)p.EntityID }
                    equals new { pl.CompanyID, ProductID = pl.ProductID } into plJoin
                from pl in plJoin.DefaultIfEmpty()
                join sl in _sellLocationService.Query() on new { CompanyID = p.CompanyID, StyleSellLocationID = pl != null ? pl.StyleSellLocationID : (Guid?)null }
                    equals new { sl.CompanyID, StyleSellLocationID = (Guid?)sl.EntityID } into sellLocJoin
                from sl in sellLocJoin.DefaultIfEmpty()
                join cl in _companyLocationService.Query() on new { CompanyID = p.CompanyID, LocationID = sl != null ? sl.LocationID : Guid.Empty }
                    equals new { cl.CompanyID, LocationID = cl.EntityID } into locJoin
                from cl in locJoin.DefaultIfEmpty()
                orderby si.Sequence
                select new StockGridRowDto
                {
                    ProductId = p.EntityID,
                    SizeId = sz.EntityID,
                    SizeDescription = sz.Description,
                    LocationId = sl != null ? sl.LocationID : Guid.Empty,
                    LocationName = cl != null ? cl.Name : null,
                    Held = pl != null ? pl.Held : 0,
                    Allocated = pl != null ? pl.Allocated : 0,
                    Available = pl != null ? pl.AvailableFirm1 : 0
                };

            return await query.ToListAsync();
        }
    }
}
