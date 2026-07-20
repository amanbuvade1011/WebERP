using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class TradingTermsService : ITradingTermsService
    {
        private readonly ITradingTermsRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public TradingTermsService(ITradingTermsRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public Task<List<TradingTermsDto>> GetAllAsync() => _repository.GetAllAsync(_currentUserService.CompanyId);
    }
}
