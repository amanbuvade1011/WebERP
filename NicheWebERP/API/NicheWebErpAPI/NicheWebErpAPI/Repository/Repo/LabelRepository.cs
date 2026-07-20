using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class LabelRepository : ILabelRepository
    {
        private readonly IEfCoreService<Label> _labelService;

        public LabelRepository(IEfCoreService<Label> labelService)
        {
            _labelService = labelService;
        }

        public async Task<List<LabelDto>> GetAllAsync(Guid companyId) =>
            await _labelService.Query()
                .Where(l => l.CompanyID == companyId)
                .OrderBy(l => l.Description)
                .Select(l => new LabelDto { Id = l.EntityID, Description = l.Description })
                .ToListAsync();
    }
}
