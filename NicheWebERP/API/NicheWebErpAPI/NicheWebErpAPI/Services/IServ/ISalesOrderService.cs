using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ISalesOrderService
    {
        Task<PagedResultDto<SalesOrderListItemDto>> GetPagedAsync(
            int page, int pageSize, int? status, Guid? firmId, DateTime? dateFrom, DateTime? dateTo);
        Task<SalesOrderDetailDto> GetByIdAsync(Guid id);
        Task<SalesOrderDetailDto> CreateAsync(CreateSalesOrderDto dto);
        Task<SalesOrderDetailDto> UpdateStatusAsync(Guid id, UpdateSalesOrderStatusDto dto);
        Task<SalesOrderDetailDto> AddLineAsync(Guid id, AddSalesOrderLineDto dto);
        Task<SalesOrderDetailDto> UpdateLineAsync(Guid id, Guid lineId, UpdateSalesOrderLineDto dto);
        Task<SalesOrderDetailDto> RemoveLineAsync(Guid id, Guid lineId);
    }
}
