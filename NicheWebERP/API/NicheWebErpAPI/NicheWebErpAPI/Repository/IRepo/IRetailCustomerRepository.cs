using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IRetailCustomerRepository
    {
        Task<PagedResultDto<RetailCustomerListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search);
        Task<Person?> GetEntityByIdAsync(Guid companyId, Guid id);
        Task AddAsync(Person person);
        Task<int> SaveChangesAsync();
    }
}
