using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    // Sales Orders live on the generic legacy TransactionBase/TransactionLine/TransactionQuantity
    // tables (EntityClassName = "WholesaleStockSalesOrder" / "StyleTransactionLine" /
    // "SalesOrderQuantity" - the only live values, confirmed 2026-07-19). See
    // docs/ai-plan/01-database-map.md.
    public class SalesOrderRepository : ISalesOrderRepository
    {
        public const string OrderClass = "WholesaleStockSalesOrder";
        public const string LineClass = "StyleTransactionLine";
        public const string QuantityClass = "SalesOrderQuantity";

        private readonly IEfCoreService<TransactionBase> _headerService;
        private readonly IEfCoreService<TransactionLine> _lineService;
        private readonly IEfCoreService<TransactionQuantity> _quantityService;
        private readonly IEfCoreService<Firm> _firmService;
        private readonly IEfCoreService<CompanyLocation> _locationService;
        private readonly IEfCoreService<Product> _productService;
        private readonly IEfCoreService<Style> _styleService;
        private readonly IEfCoreService<StyleColor> _styleColorService;
        private readonly IEfCoreService<SizewayItem> _sizewayItemService;
        private readonly IEfCoreService<Size> _sizeService;
        private readonly IEfCoreService<StylePrice> _stylePriceService;
        private readonly IEfCoreService<StyleSellLocation> _sellLocationService;
        private readonly IEfCoreService<ProductLocation> _productLocationService;
        private readonly IEfCoreService<PricePoint> _pricePointService;

        public SalesOrderRepository(
            IEfCoreService<TransactionBase> headerService,
            IEfCoreService<TransactionLine> lineService,
            IEfCoreService<TransactionQuantity> quantityService,
            IEfCoreService<Firm> firmService,
            IEfCoreService<CompanyLocation> locationService,
            IEfCoreService<Product> productService,
            IEfCoreService<Style> styleService,
            IEfCoreService<StyleColor> styleColorService,
            IEfCoreService<SizewayItem> sizewayItemService,
            IEfCoreService<Size> sizeService,
            IEfCoreService<StylePrice> stylePriceService,
            IEfCoreService<StyleSellLocation> sellLocationService,
            IEfCoreService<ProductLocation> productLocationService,
            IEfCoreService<PricePoint> pricePointService)
        {
            _headerService = headerService;
            _lineService = lineService;
            _quantityService = quantityService;
            _firmService = firmService;
            _locationService = locationService;
            _productService = productService;
            _styleService = styleService;
            _styleColorService = styleColorService;
            _sizewayItemService = sizewayItemService;
            _sizeService = sizeService;
            _stylePriceService = stylePriceService;
            _sellLocationService = sellLocationService;
            _productLocationService = productLocationService;
            _pricePointService = pricePointService;
        }

        public Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId) =>
            _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == firmId);

        public Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId) =>
            _locationService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == locationId);

        public Task<ProductDescriptor?> GetProductDescriptorAsync(Guid companyId, Guid productId) =>
            (
                from p in _productService.Query()
                where p.CompanyID == companyId && p.EntityID == productId
                join st in _styleService.Query() on new { p.CompanyID, StyleID = p.StyleID } equals new { st.CompanyID, StyleID = st.EntityID }
                join sc in _styleColorService.Query() on new { p.CompanyID, StyleColorID = p.StyleColorID } equals new { sc.CompanyID, StyleColorID = sc.EntityID }
                join si in _sizewayItemService.Query() on new { p.CompanyID, SizewayItemID = p.SizewayItemID } equals new { si.CompanyID, SizewayItemID = si.EntityID }
                join sz in _sizeService.Query() on new { si.CompanyID, si.SizeID } equals new { sz.CompanyID, SizeID = sz.EntityID }
                select new ProductDescriptor
                {
                    ProductId = p.EntityID,
                    StyleId = st.EntityID,
                    StyleCode = st.Code,
                    StyleColorId = sc.EntityID,
                    Color = sc.Color,
                    SizeDescription = sz.Description,
                    Inactive = p.Inactive
                }
            ).FirstOrDefaultAsync();

        public Task<StylePrice?> GetStylePriceAsync(Guid companyId, Guid styleId, Guid pricePointId) =>
            _stylePriceService.Query().FirstOrDefaultAsync(
                sp => sp.CompanyID == companyId && sp.StyleID == styleId && sp.PricePointID == pricePointId);

        public Task<StyleSellLocation?> GetStyleSellLocationAsync(Guid companyId, Guid styleId, Guid locationId) =>
            _sellLocationService.Query().FirstOrDefaultAsync(
                sl => sl.CompanyID == companyId && sl.StyleID == styleId && sl.LocationID == locationId);

        public Task<ProductLocation?> GetProductLocationAsync(Guid companyId, Guid productId, Guid styleSellLocationId) =>
            _productLocationService.Query().FirstOrDefaultAsync(
                pl => pl.CompanyID == companyId && pl.ProductID == productId && pl.StyleSellLocationID == styleSellLocationId);

        public async Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName)
        {
            var query = _headerService.Query().Where(h => h.CompanyID == companyId && h.EntityClassName == entityClassName);
            var max = await query.Select(h => (int?)h.DocumentNumber1).MaxAsync();
            return (max ?? 0) + 1;
        }

        public async Task<PagedResultDto<SalesOrderListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo)
        {
            var query =
                from h in _headerService.Query()
                where h.CompanyID == companyId && h.EntityClassName == OrderClass
                join f in _firmService.Query() on new { h.CompanyID, OtherPartyID = h.OtherPartyID } equals new { f.CompanyID, OtherPartyID = f.EntityID }
                select new { h, f };

            if (status.HasValue)
            {
                query = query.Where(x => x.h.Status1 == status.Value);
            }
            if (firmId.HasValue)
            {
                query = query.Where(x => x.h.OtherPartyID == firmId.Value);
            }
            if (dateFrom.HasValue)
            {
                query = query.Where(x => x.h.TransactionDate >= dateFrom.Value);
            }
            if (dateTo.HasValue)
            {
                query = query.Where(x => x.h.TransactionDate <= dateTo.Value);
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.h.TransactionDate)
                .ThenByDescending(x => x.h.DocumentNumber1)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(x => new SalesOrderListItemDto
                {
                    Id = x.h.EntityID,
                    DocumentNumber = x.h.DocumentNumber1,
                    FirmId = x.f.EntityID,
                    FirmName = x.f.TradingName,
                    OrderDate = x.h.TransactionDate,
                    CustomerReferenceNo = x.h.CustomerReferenceNo,
                    Status = x.h.Status1 ?? 0,
                    TotalQuantities = x.h.TotalQuantities1,
                    TotalAmount = x.h.Amount1
                })
                .ToListAsync();

            foreach (var item in items)
            {
                item.StatusName = ((SalesOrderStatus)item.Status).ToString();
            }

            return new PagedResultDto<SalesOrderListItemDto> { Items = items, TotalCount = totalCount, Page = page, PageSize = pageSize };
        }

        public Task<TransactionBase?> GetHeaderEntityAsync(Guid companyId, Guid id) =>
            _headerService.Query().FirstOrDefaultAsync(h => h.CompanyID == companyId && h.EntityID == id && h.EntityClassName == OrderClass);

        public async Task<SalesOrderDetailDto?> GetDetailAsync(Guid companyId, Guid id)
        {
            var header = await GetHeaderEntityAsync(companyId, id);
            if (header is null)
            {
                return null;
            }

            var firm = await _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == header.OtherPartyID);
            var location = await _locationService.Query().FirstOrDefaultAsync(l => l.CompanyID == companyId && l.EntityID == header.LocationID);
            var pricePoint = await _pricePointService.Query().FirstOrDefaultAsync(pp => pp.CompanyID == companyId && pp.EntityID == header.PricePointID);

            var dto = new SalesOrderDetailDto
            {
                Id = header.EntityID,
                DocumentNumber = header.DocumentNumber1,
                FirmId = header.OtherPartyID,
                FirmName = firm?.TradingName ?? "(unknown)",
                LocationId = header.LocationID,
                LocationName = location?.Name,
                PricePointId = header.PricePointID,
                PricePointName = pricePoint?.Name,
                OrderDate = header.TransactionDate,
                CustomerReferenceNo = header.CustomerReferenceNo,
                Narration = header.Narration,
                Status = header.Status1 ?? 0,
                SubTotalExTax = header.SubTotalStyles1,
                TaxAmount = header.TaxAmount1,
                TotalAmount = header.Amount1,
                TotalQuantities = header.TotalQuantities1
            };
            dto.StatusName = ((SalesOrderStatus)dto.Status).ToString();

            var lines =
                from tl in _lineService.Query()
                where tl.CompanyID == companyId && tl.TransactionID == id && tl.EntityClassName == LineClass
                join tq in _quantityService.Query() on new { tl.CompanyID, TransactionLineID = tl.EntityID }
                    equals new { tq.CompanyID, TransactionLineID = tq.TransactionLineID } into tqJoin
                from tq in tqJoin.DefaultIfEmpty()
                select new { tl, tq };

            var lineRows = await lines.ToListAsync();

            dto.Lines = lineRows
                .GroupBy(x => x.tl.EntityID)
                .Select(g =>
                {
                    var tl = g.First().tl;
                    var qty = g.Sum(x => x.tq?.Quantity ?? 0);
                    return new SalesOrderLineDto
                    {
                        LineId = tl.EntityID,
                        ProductId = Guid.Empty,
                        Quantity = qty,
                        UnitPriceExTax = tl.StylePriceExTax1 ?? 0,
                        UnitPriceTax = tl.StylePriceTax1 ?? 0,
                        LineTotalExTax = tl.LineAmountExTax1,
                        LineTotalTax = tl.LineTaxAmount1
                    };
                })
                .ToList();

            // Enrich each line with product descriptor (style/color/size) - the ProductLocation
            // that TransactionQuantity points at carries the ProductID, so resolve it per line.
            foreach (var lineDto in dto.Lines)
            {
                var firstQuantity = lineRows.First(x => x.tl.EntityID == lineDto.LineId).tq;
                if (firstQuantity is null)
                {
                    continue;
                }

                var productLocation = await _productLocationService.Query()
                    .FirstOrDefaultAsync(pl => pl.CompanyID == companyId && pl.EntityID == firstQuantity.ProductLocationID);
                if (productLocation?.ProductID is null)
                {
                    continue;
                }

                var descriptor = await GetProductDescriptorAsync(companyId, productLocation.ProductID.Value);
                if (descriptor is null)
                {
                    continue;
                }

                lineDto.ProductId = descriptor.ProductId;
                lineDto.StyleCode = descriptor.StyleCode;
                lineDto.Color = descriptor.Color;
                lineDto.SizeDescription = descriptor.SizeDescription;
            }

            return dto;
        }

        public Task<List<TransactionLine>> GetLineEntitiesAsync(Guid companyId, Guid headerId) =>
            _lineService.Query()
                .Where(l => l.CompanyID == companyId && l.TransactionID == headerId && l.EntityClassName == LineClass)
                .ToListAsync();

        public Task<TransactionLine?> GetLineEntityAsync(Guid companyId, Guid headerId, Guid lineId) =>
            _lineService.Query().FirstOrDefaultAsync(
                l => l.CompanyID == companyId && l.TransactionID == headerId && l.EntityID == lineId && l.EntityClassName == LineClass);

        public Task<List<TransactionQuantity>> GetQuantityEntitiesAsync(Guid companyId, Guid lineId) =>
            _quantityService.Query()
                .Where(q => q.CompanyID == companyId && q.TransactionLineID == lineId && q.EntityClassName == QuantityClass)
                .ToListAsync();

        public Task AddHeaderAsync(TransactionBase header) => _headerService.AddAsync(header);
        public Task AddLineAsync(TransactionLine line) => _lineService.AddAsync(line);
        public Task AddQuantityAsync(TransactionQuantity quantity) => _quantityService.AddAsync(quantity);
        public void UpdateHeader(TransactionBase header) => _headerService.Update(header);
        public void UpdateLine(TransactionLine line) => _lineService.Update(line);
        public void RemoveLine(TransactionLine line) => _lineService.Remove(line);
        public void RemoveQuantities(IEnumerable<TransactionQuantity> quantities) => _quantityService.RemoveRange(quantities);

        public Task<int> SaveChangesAsync() => _headerService.SaveChangesAsync();
    }
}
