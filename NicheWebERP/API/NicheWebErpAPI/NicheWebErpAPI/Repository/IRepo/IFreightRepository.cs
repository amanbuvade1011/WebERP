using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IFreightRepository
    {
        Task<CompanyLocation?> GetLocationAsync(Guid companyId, Guid locationId);
        Task<List<FreightItem>> GetItemsAsync(Guid companyId, Guid freightCalculatorId);
    }
}
