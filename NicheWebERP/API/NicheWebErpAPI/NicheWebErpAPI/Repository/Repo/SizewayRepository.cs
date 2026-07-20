using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI;

namespace NicheWebErpAPI.Repository.Repo
{
    public class SizewayRepository : ISizewayRepository
    {
        private readonly IEfCoreService<Sizeway> _sizewayService;
        private readonly IEfCoreService<SizewayItem> _sizewayItemService;
        private readonly IEfCoreService<Size> _sizeService;

        public SizewayRepository(
            IEfCoreService<Sizeway> sizewayService,
            IEfCoreService<SizewayItem> sizewayItemService,
            IEfCoreService<Size> sizeService)
        {
            _sizewayService = sizewayService;
            _sizewayItemService = sizewayItemService;
            _sizeService = sizeService;
        }

        public async Task<List<SizewayDto>> GetAllAsync(Guid companyId)
        {
            var sizeways = await _sizewayService.Query()
                .Where(sw => sw.CompanyID == companyId)
                .OrderBy(sw => sw.Description)
                .ToListAsync();

            var result = new List<SizewayDto>();
            foreach (var sw in sizeways)
            {
                result.Add(new SizewayDto
                {
                    Id = sw.EntityID,
                    Description = sw.Description,
                    ExcludeRetailSearch = sw.ExcludeRetailSearch,
                    Sizes = await GetSizeItemDtosAsync(companyId, sw.EntityID)
                });
            }
            return result;
        }

        private async Task<List<SizewayItemDto>> GetSizeItemDtosAsync(Guid companyId, Guid sizewayId)
        {
            var query =
                from si in _sizewayItemService.Query()
                join sz in _sizeService.Query() on new { si.CompanyID, si.SizeID }
                    equals new { sz.CompanyID, SizeID = sz.EntityID }
                where si.CompanyID == companyId && si.SizewayID == sizewayId
                orderby si.Sequence
                select new SizewayItemDto { SizeId = sz.EntityID, SizeDescription = sz.Description, Sequence = si.Sequence };

            return await query.ToListAsync();
        }

        public Task<Sizeway?> GetEntityByIdAsync(Guid companyId, Guid id) =>
            _sizewayService.Query().FirstOrDefaultAsync(sw => sw.CompanyID == companyId && sw.EntityID == id);

        public Task<List<SizewayItem>> GetItemsAsync(Guid companyId, Guid sizewayId) =>
            _sizewayItemService.Query()
                .Where(si => si.CompanyID == companyId && si.SizewayID == sizewayId)
                .OrderBy(si => si.Sequence)
                .ToListAsync();

        public Task AddAsync(Sizeway sizeway) => _sizewayService.AddAsync(sizeway);

        public Task AddItemAsync(SizewayItem item) => _sizewayItemService.AddAsync(item);

        public void RemoveItem(SizewayItem item) => _sizewayItemService.Remove(item);

        public Task<int> SaveChangesAsync() => _sizewayService.SaveChangesAsync();
    }
}
