using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class SizewayService : ISizewayService
    {
        private readonly ISizewayRepository _sizewayRepository;
        private readonly ICurrentUserService _currentUserService;

        public SizewayService(ISizewayRepository sizewayRepository, ICurrentUserService currentUserService)
        {
            _sizewayRepository = sizewayRepository;
            _currentUserService = currentUserService;
        }

        public Task<List<SizewayDto>> GetAllAsync() =>
            _sizewayRepository.GetAllAsync(_currentUserService.CompanyId);

        public async Task<SizewayDto> CreateAsync(CreateSizewayDto dto)
        {
            if (dto.SizeIds.Count == 0)
            {
                throw new InvalidOperationException("A sizeway needs at least one size.");
            }

            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var sizeway = new Sizeway
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                Description = dto.Description,
                ExcludeRetailSearch = dto.ExcludeRetailSearch,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = updatedById
            };
            await _sizewayRepository.AddAsync(sizeway);

            for (var i = 0; i < dto.SizeIds.Count; i++)
            {
                await _sizewayRepository.AddItemAsync(new SizewayItem
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    SizewayID = sizeway.EntityID,
                    SizeID = dto.SizeIds[i],
                    Sequence = i + 1,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedByID = updatedById
                });
            }

            await _sizewayRepository.SaveChangesAsync();

            return (await _sizewayRepository.GetAllAsync(companyId)).First(s => s.Id == sizeway.EntityID);
        }

        // Full ordered replacement - simpler and less error-prone than diffing existing
        // sequence numbers against a reordered list.
        public async Task<SizewayDto> UpdateSizeSequenceAsync(Guid sizewayId, UpdateSizeSequenceDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var sizeway = await _sizewayRepository.GetEntityByIdAsync(companyId, sizewayId)
                ?? throw new KeyNotFoundException($"Sizeway {sizewayId} not found.");

            var existingItems = await _sizewayRepository.GetItemsAsync(companyId, sizewayId);
            foreach (var item in existingItems)
            {
                _sizewayRepository.RemoveItem(item);
            }

            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;
            for (var i = 0; i < dto.SizeIds.Count; i++)
            {
                await _sizewayRepository.AddItemAsync(new SizewayItem
                {
                    CompanyID = companyId,
                    EntityID = Guid.NewGuid(),
                    SizewayID = sizewayId,
                    SizeID = dto.SizeIds[i],
                    Sequence = i + 1,
                    LastUpdated = DateTime.UtcNow,
                    UpdatedByID = updatedById
                });
            }

            await _sizewayRepository.SaveChangesAsync();

            return (await _sizewayRepository.GetAllAsync(companyId)).First(s => s.Id == sizewayId);
        }
    }
}
