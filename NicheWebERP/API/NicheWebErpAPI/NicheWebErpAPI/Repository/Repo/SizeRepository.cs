using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class SizeRepository : ISizeRepository
    {
        private readonly IEfCoreService<Size> _sizeService;

        public SizeRepository(IEfCoreService<Size> sizeService)
        {
            _sizeService = sizeService;
        }

        public async Task<List<SizeDto>> GetAllAsync(Guid companyId) =>
            await _sizeService.Query()
                .Where(s => s.CompanyID == companyId)
                .OrderBy(s => s.Description)
                .Select(s => new SizeDto { Id = s.EntityID, Description = s.Description })
                .ToListAsync();
    }
}
