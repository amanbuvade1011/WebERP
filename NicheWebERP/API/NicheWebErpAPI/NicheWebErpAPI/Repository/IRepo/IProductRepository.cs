using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IProductRepository
    {
        Task<IEnumerable<ProductListItemDto>> GetAllProductsAsync();
    }
}
