using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IStyleService
    {
        Task<PagedResultDto<StyleListItemDto>> GetPagedAsync(
            int page, int pageSize, string? search,
            Guid? categoryId, Guid? labelId, Guid? rangeId, bool? inactive);

        Task<StyleDetailDto> GetByIdAsync(Guid id);
        Task<StyleDetailDto> CreateAsync(CreateStyleDto dto);
        Task<StyleDetailDto> UpdateAsync(Guid id, UpdateStyleDto dto);
        Task<StyleColorDto> AddColorAsync(Guid styleId, AddColorDto dto);
    }
}
