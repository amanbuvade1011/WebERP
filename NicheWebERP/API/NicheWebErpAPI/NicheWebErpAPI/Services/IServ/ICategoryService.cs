using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface ICategoryService
    {
        Task<List<CategoryNodeDto>> GetTreeAsync();
        Task<CategoryNodeDto> CreateAsync(CreateCategoryDto dto);
    }
}
