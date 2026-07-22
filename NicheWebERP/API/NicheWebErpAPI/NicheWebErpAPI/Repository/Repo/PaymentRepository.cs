using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    // Payments reuse the "Payment" EntityClassName - already a live legacy value (13,949 rows,
    // though all unrelated legacy retail/POS activity, confirmed 2026-07-20). Allocation to an
    // invoice goes through FinancialAllocation (0 live rows - net-new usage). See
    // docs/ai-plan/sprints/sprint-06-invoicing-payments.md.
    public class PaymentRepository : IPaymentRepository
    {
        public const string PaymentClass = "Payment";

        private readonly IEfCoreService<TransactionBase> _headerService;
        private readonly IEfCoreService<PaymentMethod> _paymentMethodService;
        private readonly IEfCoreService<FinancialAllocation> _allocationService;

        public PaymentRepository(
            IEfCoreService<TransactionBase> headerService,
            IEfCoreService<PaymentMethod> paymentMethodService,
            IEfCoreService<FinancialAllocation> allocationService)
        {
            _headerService = headerService;
            _paymentMethodService = paymentMethodService;
            _allocationService = allocationService;
        }

        public Task<TransactionBase?> GetInvoiceHeaderAsync(Guid companyId, Guid invoiceId) =>
            _headerService.Query().FirstOrDefaultAsync(
                h => h.CompanyID == companyId && h.EntityID == invoiceId && h.EntityClassName == InvoiceRepository.InvoiceClass);

        public Task<PaymentMethod?> GetPaymentMethodAsync(Guid companyId, Guid paymentMethodId) =>
            _paymentMethodService.Query().FirstOrDefaultAsync(pm => pm.CompanyID == companyId && pm.EntityID == paymentMethodId);

        public async Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName)
        {
            var query = _headerService.Query().Where(h => h.CompanyID == companyId && h.EntityClassName == entityClassName);
            var max = await query.Select(h => (int?)h.DocumentNumber1).MaxAsync();
            return (max ?? 0) + 1;
        }

        public Task AddPaymentAsync(TransactionBase payment) => _headerService.AddAsync(payment);
        public Task AddAllocationAsync(FinancialAllocation allocation) => _allocationService.AddAsync(allocation);

        public Task<int> SaveChangesAsync() => _headerService.SaveChangesAsync();
    }
}
