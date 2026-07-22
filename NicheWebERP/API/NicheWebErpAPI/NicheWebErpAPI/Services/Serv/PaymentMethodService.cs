using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class PaymentMethodService : IPaymentMethodService
    {
        private readonly IPaymentMethodRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public PaymentMethodService(IPaymentMethodRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public Task<List<PaymentMethodDto>> GetAllAsync() => _repository.GetAllWholesaleAsync(_currentUserService.CompanyId);
    }
}
