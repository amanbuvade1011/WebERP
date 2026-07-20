using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IRetailCustomerService
    {
        Task<PagedResultDto<RetailCustomerListItemDto>> GetPagedAsync(int page, int pageSize, string? search);
        Task<RetailCustomerDetailDto> CreateAsync(CreateRetailCustomerDto dto);
    }
}
