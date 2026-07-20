using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class StyleService : IStyleService
    {
        private readonly IStyleRepository _styleRepository;
        private readonly ICurrentUserService _currentUserService;

        public StyleService(IStyleRepository styleRepository, ICurrentUserService currentUserService)
        {
            _styleRepository = styleRepository;
            _currentUserService = currentUserService;
        }

        public Task<PagedResultDto<StyleListItemDto>> GetPagedAsync(
            int page, int pageSize, string? search,
            Guid? categoryId, Guid? labelId, Guid? rangeId, bool? inactive)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _styleRepository.GetPagedAsync(
                _currentUserService.CompanyId, page, pageSize, search, categoryId, labelId, rangeId, inactive);
        }

        public async Task<StyleDetailDto> GetByIdAsync(Guid id)
        {
            var companyId = _currentUserService.CompanyId;
            return await _styleRepository.GetDetailByIdAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Style {id} not found.");
        }

        public async Task<StyleDetailDto> CreateAsync(CreateStyleDto dto)
        {
            var companyId = _currentUserService.CompanyId;

            if (!await _styleRepository.SizewayExistsAsync(companyId, dto.SizewayId))
            {
                throw new InvalidOperationException($"Sizeway {dto.SizewayId} does not exist.");
            }
            if (dto.CategoryId.HasValue && !await _styleRepository.CategoryExistsAsync(companyId, dto.CategoryId.Value))
            {
                throw new InvalidOperationException($"Category {dto.CategoryId} does not exist.");
            }
            if (dto.LabelId.HasValue && !await _styleRepository.LabelExistsAsync(companyId, dto.LabelId.Value))
            {
                throw new InvalidOperationException($"Label {dto.LabelId} does not exist.");
            }
            if (!await _styleRepository.RangeExistsAsync(companyId, dto.RangeId))
            {
                throw new InvalidOperationException($"Range {dto.RangeId} does not exist.");
            }

            var style = new Style
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                Code = dto.Code,
                Description = dto.Description,
                WebDescription = dto.WebDescription,
                Weight = dto.Weight,
                SizewayID = dto.SizewayId,
                CategoryID = dto.CategoryId,
                LabelID = dto.LabelId,
                RangeID = dto.RangeId,
                DeliveryPeriod = dto.DeliveryPeriod,
                Inactive = false,
                SortRankMajor = 0,
                SortRankMinor = 0,
                NonStock = false,
                AllowManualPrice = false,
                AllowManualDescription = false,
                DoPopUp = false,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty
            };

            await _styleRepository.AddAsync(style);
            await _styleRepository.SaveChangesAsync();

            return await GetByIdAsync(style.EntityID);
        }

        public async Task<StyleDetailDto> UpdateAsync(Guid id, UpdateStyleDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var style = await _styleRepository.GetEntityByIdAsync(companyId, id)
                ?? throw new KeyNotFoundException($"Style {id} not found.");

            if (!await _styleRepository.SizewayExistsAsync(companyId, dto.SizewayId))
            {
                throw new InvalidOperationException($"Sizeway {dto.SizewayId} does not exist.");
            }
            if (dto.CategoryId.HasValue && !await _styleRepository.CategoryExistsAsync(companyId, dto.CategoryId.Value))
            {
                throw new InvalidOperationException($"Category {dto.CategoryId} does not exist.");
            }
            if (dto.LabelId.HasValue && !await _styleRepository.LabelExistsAsync(companyId, dto.LabelId.Value))
            {
                throw new InvalidOperationException($"Label {dto.LabelId} does not exist.");
            }
            if (!await _styleRepository.RangeExistsAsync(companyId, dto.RangeId))
            {
                throw new InvalidOperationException($"Range {dto.RangeId} does not exist.");
            }

            style.Description = dto.Description;
            style.WebDescription = dto.WebDescription;
            style.Weight = dto.Weight;
            style.SizewayID = dto.SizewayId;
            style.CategoryID = dto.CategoryId;
            style.LabelID = dto.LabelId;
            style.RangeID = dto.RangeId;
            style.DeliveryPeriod = dto.DeliveryPeriod;
            style.Inactive = dto.Inactive;
            style.LastUpdated = DateTime.UtcNow;
            style.UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : style.UpdatedByID;

            _styleRepository.Update(style);
            await _styleRepository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<StyleColorDto> AddColorAsync(Guid styleId, AddColorDto dto)
        {
            var companyId = _currentUserService.CompanyId;

            if (await _styleRepository.GetEntityByIdAsync(companyId, styleId) is null)
            {
                throw new KeyNotFoundException($"Style {styleId} not found.");
            }
            if (await _styleRepository.ColorExistsAsync(companyId, styleId, dto.Color))
            {
                throw new InvalidOperationException($"Style already has a color named '{dto.Color}'.");
            }

            var color = new StyleColor
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                StyleID = styleId,
                Color = dto.Color,
                RgbValue = dto.RgbValue,
                Inactive = false,
                LastUpdated = DateTime.UtcNow,
                UpdatedByID = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty
            };

            await _styleRepository.AddColorAsync(color);
            await _styleRepository.SaveChangesAsync();

            return new StyleColorDto { Id = color.EntityID, Color = color.Color, RgbValue = color.RgbValue, Inactive = false };
        }
    }
}
