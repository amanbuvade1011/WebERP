using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface ICompanyLocationRepository
    {
        Task<Guid> GetMasterCompanyIdAsync();
        Task<CompanyLocation?> GetMasterCompanyAsync();
        Task<IEnumerable<CompanyLocation>> GetAllByCompanyAsync(Guid companyId);
        Task<CompanyLocation?> GetByIdAsync(Guid companyId, Guid entityId);
        Task AddAsync(CompanyLocation location);
        void Update(CompanyLocation location);
        Task<int> SaveChangesAsync();
    }
}
