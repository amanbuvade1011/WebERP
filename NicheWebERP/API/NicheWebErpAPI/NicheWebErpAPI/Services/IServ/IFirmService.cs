using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IFirmService
    {
        Task<PagedResultDto<FirmListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? entityClassName);
        Task<FirmDetailDto> GetByIdAsync(Guid id);
        Task<FirmDetailDto> CreateAsync(CreateFirmDto dto);
        Task<FirmDetailDto> UpdateAsync(Guid id, UpdateFirmDto dto);
    }
}
