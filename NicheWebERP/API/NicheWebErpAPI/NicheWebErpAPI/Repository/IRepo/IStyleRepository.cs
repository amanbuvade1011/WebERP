using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IStyleRepository
    {
        Task<PagedResultDto<StyleListItemDto>> GetPagedAsync(
            Guid companyId, int page, int pageSize, string? search,
            Guid? categoryId, Guid? labelId, Guid? rangeId, bool? inactive);

        Task<Style?> GetEntityByIdAsync(Guid companyId, Guid id);
        Task<StyleDetailDto?> GetDetailByIdAsync(Guid companyId, Guid id);
        Task AddAsync(Style style);
        void Update(Style style);
        Task<int> SaveChangesAsync();

        Task<List<StyleColorDto>> GetColorsAsync(Guid companyId, Guid styleId);
        Task<bool> ColorExistsAsync(Guid companyId, Guid styleId, string color);
        Task AddColorAsync(StyleColor color);

        Task<bool> SizewayExistsAsync(Guid companyId, Guid sizewayId);
        Task<bool> CategoryExistsAsync(Guid companyId, Guid categoryId);
        Task<bool> LabelExistsAsync(Guid companyId, Guid labelId);
        Task<bool> RangeExistsAsync(Guid companyId, Guid rangeId);
    }
}
