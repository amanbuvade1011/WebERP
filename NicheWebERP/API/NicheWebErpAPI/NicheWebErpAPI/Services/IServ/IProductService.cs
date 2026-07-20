using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IProductService
    {
        Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(
            int page, int pageSize, string? search, Guid? categoryId, Guid? labelId);
    }
}
