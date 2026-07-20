using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IProductRepository
    {
        Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(
            Guid companyId, int page, int pageSize, string? search, Guid? categoryId, Guid? labelId);
    }
}
