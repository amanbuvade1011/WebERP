using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class LocationService : ILocationService
    {
        private readonly ICompanyLocationRepository _companyLocationRepository;
        private readonly ICurrentUserService _currentUserService;

        public LocationService(
            ICompanyLocationRepository companyLocationRepository,
            ICurrentUserService currentUserService)
        {
            _companyLocationRepository = companyLocationRepository;
            _currentUserService = currentUserService;
        }

        public async Task<IEnumerable<LocationDto>> GetAllLocationsAsync()
        {
            var companyId = await _companyLocationRepository.GetMasterCompanyIdAsync();
            var locations = await _companyLocationRepository.GetAllByCompanyAsync(companyId);
            return locations.Select(ToDto);
        }

        // Nested-set insert: adds the new location as the last child of the given parent.
        // Standard "insert node" algorithm - see docs/ai-plan/01-database-map.md /
        // 02-api-plan.md for why this is the trickiest logic in the Company module.
        // Deliberately lives here (service layer), not in the controller or repository.
        public async Task<LocationDto> CreateLocationAsync(CreateLocationDto dto)
        {
            var companyId = await _companyLocationRepository.GetMasterCompanyIdAsync();

            var parent = await _companyLocationRepository.GetByIdAsync(companyId, dto.ParentId)
                ?? throw new KeyNotFoundException($"Parent location {dto.ParentId} not found.");

            // Defensive: the legacy data can have an un-initialized/degenerate nested-set root
            // (AdministrationLeftTag = AdministrationRightTag = 0 - confirmed live in this
            // database on 2026-07-19, see docs/ai-plan/01-database-map.md). A valid nested-set
            // node always has RightTag > LeftTag; self-heal to a fresh single-node tree (1, 2)
            // before running the standard insert algorithm below, rather than propagating the
            // corruption into every location created under it.
            if (parent.AdministrationRightTag <= parent.AdministrationLeftTag)
            {
                parent.AdministrationLeftTag = 1;
                parent.AdministrationRightTag = 2;
                _companyLocationRepository.Update(parent);
            }

            var parentRightTag = parent.AdministrationRightTag;

            // All rows for this company are loaded and mutated in memory, then saved in one
            // batch - correct per the standard nested-set "insert" algorithm, and cheap at the
            // location counts this system actually has (single digits to low hundreds).
            var allLocations = (await _companyLocationRepository.GetAllByCompanyAsync(companyId)).ToList();

            foreach (var loc in allLocations.Where(l => l.AdministrationRightTag >= parentRightTag))
            {
                loc.AdministrationRightTag += 2;
                _companyLocationRepository.Update(loc);
            }

            foreach (var loc in allLocations.Where(l => l.AdministrationLeftTag >= parentRightTag))
            {
                loc.AdministrationLeftTag += 2;
                _companyLocationRepository.Update(loc);
            }

            var newLocation = new CompanyLocation
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                EntityClassName = "Location",
                Name = dto.Name,
                Code = dto.Code,
                AdministrationParentID = parent.EntityID,
                AdministrationLeftTag = parentRightTag,
                AdministrationRightTag = parentRightTag + 1,
                CountryID = dto.CountryId ?? parent.CountryID,
                LanguageID = parent.LanguageID,
                FreightID = parent.FreightID,
                Inactive = false,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated
                    ? _currentUserService.LegacyPersonId
                    : Guid.Empty
            };

            await _companyLocationRepository.AddAsync(newLocation);
            await _companyLocationRepository.SaveChangesAsync();

            return ToDto(newLocation);
        }

        private static LocationDto ToDto(CompanyLocation c) => new()
        {
            Id = c.EntityID,
            Name = c.Name,
            Code = c.Code,
            ParentId = c.AdministrationParentID == Guid.Empty ? null : c.AdministrationParentID,
            Inactive = c.Inactive
        };
    }
}
