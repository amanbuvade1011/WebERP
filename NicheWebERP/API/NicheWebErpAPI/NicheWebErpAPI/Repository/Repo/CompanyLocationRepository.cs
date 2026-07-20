using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class CompanyLocationRepository : ICompanyLocationRepository
    {
        private readonly IEfCoreService<CompanyLocation> _companyLocationService;

        public CompanyLocationRepository(IEfCoreService<CompanyLocation> companyLocationService)
        {
            _companyLocationService = companyLocationService;
        }

        public async Task<Guid> GetMasterCompanyIdAsync()
        {
            var master = await _companyLocationService.Query()
                .Where(c => c.EntityClassName == "MasterCompany")
                .Select(c => c.CompanyID)
                .FirstOrDefaultAsync();

            if (master == Guid.Empty)
            {
                throw new InvalidOperationException(
                    "No CompanyLocation row with EntityClassName = 'MasterCompany' was found.");
            }

            return master;
        }

        public Task<CompanyLocation?> GetMasterCompanyAsync() =>
            _companyLocationService.Query()
                .FirstOrDefaultAsync(c => c.EntityClassName == "MasterCompany");

        public async Task<IEnumerable<CompanyLocation>> GetAllByCompanyAsync(Guid companyId) =>
            await _companyLocationService.Query()
                .Where(c => c.CompanyID == companyId)
                .OrderBy(c => c.AdministrationLeftTag)
                .ToListAsync();

        public Task<CompanyLocation?> GetByIdAsync(Guid companyId, Guid entityId) =>
            _companyLocationService.Query()
                .FirstOrDefaultAsync(c => c.CompanyID == companyId && c.EntityID == entityId);

        public Task AddAsync(CompanyLocation location) => _companyLocationService.AddAsync(location);

        public void Update(CompanyLocation location) => _companyLocationService.Update(location);

        public Task<int> SaveChangesAsync() => _companyLocationService.SaveChangesAsync();
    }
}
