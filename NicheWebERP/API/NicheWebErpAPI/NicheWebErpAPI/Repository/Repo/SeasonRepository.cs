using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class SeasonRepository : ISeasonRepository
    {
        private readonly IEfCoreService<Season> _seasonService;

        public SeasonRepository(IEfCoreService<Season> seasonService)
        {
            _seasonService = seasonService;
        }

        public async Task<List<SeasonDto>> GetAllAsync(Guid companyId) =>
            await _seasonService.Query()
                .Where(s => s.CompanyID == companyId)
                .OrderBy(s => s.Description)
                .Select(s => new SeasonDto { Id = s.EntityID, Description = s.Description, Code = s.Code })
                .ToListAsync();
    }
}
