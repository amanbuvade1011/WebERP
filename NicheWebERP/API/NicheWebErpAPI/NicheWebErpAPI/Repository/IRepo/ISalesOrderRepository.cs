using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    // Descriptor used to resolve a Product back to its Style/Color/Size for pricing and display -
    // Product only stores logical FKs (StyleID/StyleColorID/SizewayItemID), so every consumer
    // needs to join through Style/StyleColor/SizewayItem/Size the same way. See ProductGenerationRepository
    // for the same join pattern used by the Stock Grid.
    public class ProductDescriptor
    {
        public Guid ProductId { get; set; }
        public Guid StyleId { get; set; }
        public string StyleCode { get; set; } = null!;
        public Guid StyleColorId { get; set; }
        public string Color { get; set; } = null!;
        public string SizeDescription { get; set; } = null!;
        public bool? Inactive { get; set; }
    }

    public interface ISalesOrderRepository
    {
        Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId);
        Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId);
        Task<ProductDescriptor?> GetProductDescriptorAsync(Guid companyId, Guid productId);
        Task<StylePrice?> GetStylePriceAsync(Guid companyId, Guid styleId, Guid pricePointId);
        Task<StyleSellLocation?> GetStyleSellLocationAsync(Guid companyId, Guid styleId, Guid locationId);
        Task<ProductLocation?> GetProductLocationAsync(Guid companyId, Guid productId, Guid styleSellLocationId);

        // DocumentNumber1 is not a DB identity column (confirmed live 2026-07-19) - the
        // application must generate it. MAX+1 scoped to (CompanyID, EntityClassName); this has a
        // known small race window under concurrent creates, acceptable for now - see
        // docs/ai-plan/01-database-map.md.
        Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName);

        Task<PagedResultDto<SalesOrderListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo);
        Task<TransactionBase?> GetHeaderEntityAsync(Guid companyId, Guid id);
        Task<SalesOrderDetailDto?> GetDetailAsync(Guid companyId, Guid id);
        Task<List<TransactionLine>> GetLineEntitiesAsync(Guid companyId, Guid headerId);
        Task<TransactionLine?> GetLineEntityAsync(Guid companyId, Guid headerId, Guid lineId);
        Task<List<TransactionQuantity>> GetQuantityEntitiesAsync(Guid companyId, Guid lineId);

        Task AddHeaderAsync(TransactionBase header);
        Task AddLineAsync(TransactionLine line);
        Task AddQuantityAsync(TransactionQuantity quantity);
        void UpdateHeader(TransactionBase header);
        void UpdateLine(TransactionLine line);
        void RemoveLine(TransactionLine line);
        void RemoveQuantities(IEnumerable<TransactionQuantity> quantities);

        Task<int> SaveChangesAsync();
    }
}
