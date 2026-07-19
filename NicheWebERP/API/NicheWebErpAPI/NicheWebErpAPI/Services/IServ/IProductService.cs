using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IProductService
    {
        Task<IEnumerable<ProductListItemDto>> GetAllProductsAsync();
    }
}
