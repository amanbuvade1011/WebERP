using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class SeasonService : ISeasonService
    {
        private readonly ISeasonRepository _seasonRepository;
        private readonly ICurrentUserService _currentUserService;

        public SeasonService(ISeasonRepository seasonRepository, ICurrentUserService currentUserService)
        {
            _seasonRepository = seasonRepository;
            _currentUserService = currentUserService;
        }

        public Task<List<SeasonDto>> GetAllAsync() => _seasonRepository.GetAllAsync(_currentUserService.CompanyId);
    }
}
