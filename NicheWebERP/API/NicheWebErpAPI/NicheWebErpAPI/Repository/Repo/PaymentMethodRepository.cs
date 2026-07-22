using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly IEfCoreService<Models.PaymentMethod> _paymentMethodService;

        public PaymentMethodRepository(IEfCoreService<Models.PaymentMethod> paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        // Filtered to AllowWholesale - this sprint (and Sprint 05) is wholesale-only scope.
        public async Task<List<PaymentMethodDto>> GetAllWholesaleAsync(Guid companyId) =>
            await _paymentMethodService.Query()
                .Where(pm => pm.CompanyID == companyId && pm.AllowWholesale)
                .OrderBy(pm => pm.Description)
                .Select(pm => new PaymentMethodDto
                {
                    Id = pm.EntityID,
                    Description = pm.Description,
                    PaymentMethodType = pm.PaymentMethodType1,
                    SurchargePercentage = pm.SurchargePercentage
                })
                .ToListAsync();
    }
}
