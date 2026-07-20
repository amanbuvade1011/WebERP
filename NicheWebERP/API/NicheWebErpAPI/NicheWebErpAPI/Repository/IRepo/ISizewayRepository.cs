using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ISizewayRepository
    {
        Task<List<SizewayDto>> GetAllAsync(Guid companyId);
        Task<Sizeway?> GetEntityByIdAsync(Guid companyId, Guid id);
        Task<List<SizewayItem>> GetItemsAsync(Guid companyId, Guid sizewayId);
        Task AddAsync(Sizeway sizeway);
        Task AddItemAsync(SizewayItem item);
        void RemoveItem(SizewayItem item);
        Task<int> SaveChangesAsync();
    }
}
