using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class SizeService : ISizeService
    {
        private readonly ISizeRepository _sizeRepository;
        private readonly ICurrentUserService _currentUserService;

        public SizeService(ISizeRepository sizeRepository, ICurrentUserService currentUserService)
        {
            _sizeRepository = sizeRepository;
            _currentUserService = currentUserService;
        }

        public Task<List<SizeDto>> GetAllAsync() => _sizeRepository.GetAllAsync(_currentUserService.CompanyId);
    }
}
