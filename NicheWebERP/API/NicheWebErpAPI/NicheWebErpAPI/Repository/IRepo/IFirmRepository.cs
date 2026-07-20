using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IFirmRepository
    {
        Task<PagedResultDto<FirmListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search, string? entityClassName);
        Task<Firm?> GetEntityByIdAsync(Guid companyId, Guid id);
        Task<FirmDetailDto?> GetDetailByIdAsync(Guid companyId, Guid id);
        Task AddAsync(Firm firm);
        void Update(Firm firm);
        Task<int> SaveChangesAsync();
    }
}
