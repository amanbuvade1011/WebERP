using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IPaymentRepository
    {
        Task<TransactionBase?> GetInvoiceHeaderAsync(Guid companyId, Guid invoiceId);
        Task<PaymentMethod?> GetPaymentMethodAsync(Guid companyId, Guid paymentMethodId);
        Task<int> GetNextDocumentNumberAsync(Guid companyId, string entityClassName);

        Task AddPaymentAsync(TransactionBase payment);
        Task AddAllocationAsync(FinancialAllocation allocation);

        Task<int> SaveChangesAsync();
    }
}
