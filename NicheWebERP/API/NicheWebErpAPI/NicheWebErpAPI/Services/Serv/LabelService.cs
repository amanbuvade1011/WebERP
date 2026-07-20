using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class LabelService : ILabelService
    {
        private readonly ILabelRepository _labelRepository;
        private readonly ICurrentUserService _currentUserService;

        public LabelService(ILabelRepository labelRepository, ICurrentUserService currentUserService)
        {
            _labelRepository = labelRepository;
            _currentUserService = currentUserService;
        }

        public Task<List<LabelDto>> GetAllAsync() => _labelRepository.GetAllAsync(_currentUserService.CompanyId);
    }
}
