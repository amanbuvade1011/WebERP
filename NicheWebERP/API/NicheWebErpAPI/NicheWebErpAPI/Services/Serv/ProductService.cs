using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ICurrentUserService _currentUserService;

        public ProductService(IProductRepository productRepository, ICurrentUserService currentUserService)
        {
            _productRepository = productRepository;
            _currentUserService = currentUserService;
        }

        public Task<PagedResultDto<ProductListItemDto>> GetAllProductsAsync(
            int page, int pageSize, string? search, Guid? categoryId, Guid? labelId)
        {
            page = page < 1 ? 1 : page;
            pageSize = pageSize is < 1 or > 200 ? 25 : pageSize;
            return _productRepository.GetAllProductsAsync(
                _currentUserService.CompanyId, page, pageSize, search, categoryId, labelId);
        }
    }
}
