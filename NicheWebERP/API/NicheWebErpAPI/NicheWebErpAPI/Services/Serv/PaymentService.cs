using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Repository.Repo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class PaymentService : IPaymentService
    {
        private readonly IPaymentRepository _repository;
        private readonly IInvoiceService _invoiceService;
        private readonly ICurrentUserService _currentUserService;

        public PaymentService(IPaymentRepository repository, IInvoiceService invoiceService, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _invoiceService = invoiceService;
            _currentUserService = currentUserService;
        }

        public async Task<InvoiceDetailDto> RecordPaymentAsync(RecordPaymentDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            if (dto.Amount <= 0)
            {
                throw new InvalidOperationException("Payment amount must be greater than zero.");
            }

            // Acceptance: verify the invoice exists and re-derive its current remaining balance
            // from InvoiceService (the authoritative Paid/Pending/Overdue computation) rather than
            // duplicating the allocation-sum logic here.
            var invoice = await _invoiceService.GetByIdAsync(dto.InvoiceId);
            if (dto.Amount > invoice.RemainingAmount)
            {
                throw new InvalidOperationException(
                    $"Payment of {dto.Amount:C} exceeds the invoice's remaining balance of {invoice.RemainingAmount:C}.");
            }

            var paymentMethod = await _repository.GetPaymentMethodAsync(companyId, dto.PaymentMethodId)
                ?? throw new KeyNotFoundException($"Payment method {dto.PaymentMethodId} not found.");

            var invoiceHeader = await _repository.GetInvoiceHeaderAsync(companyId, dto.InvoiceId)
                ?? throw new KeyNotFoundException($"Invoice {dto.InvoiceId} not found.");

            var now = DateTime.UtcNow;
            var documentNumber = await _repository.GetNextDocumentNumberAsync(companyId, PaymentRepository.PaymentClass);

            var payment = new TransactionBase
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                EntityClassName = PaymentRepository.PaymentClass,
                LaneID = Guid.Empty,
                Narration = dto.Narration,
                TransactionDate = now,
                OtherPartyID = invoiceHeader.OtherPartyID,
                DocumentNumber1 = documentNumber,
                LocationID = invoiceHeader.LocationID,
                Amount1 = dto.Amount,
                PricePointID = invoiceHeader.PricePointID,
                PaymentMethodID = paymentMethod.EntityID,
                AgentID = Guid.Empty,
                RepresentativeID = Guid.Empty,
                IsDraft = false,
                TaxAmount1 = 0,
                NumLines1 = 0,
                TotalQuantities1 = 0,
                SubTotalStyles1 = 0,
                SubTotalStylesTax1 = 0,
                EmployeeID = updatedById,
                LastUpdated = now,
                UpdatedByID = updatedById
            };
            await _repository.AddPaymentAsync(payment);

            var allocation = new FinancialAllocation
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                TransactionFromID = payment.EntityID,
                TransactionToID = dto.InvoiceId,
                Amount = dto.Amount,
                LastUpdated = now,
                UpdatedByID = updatedById
            };
            await _repository.AddAllocationAsync(allocation);

            await _repository.SaveChangesAsync();

            return await _invoiceService.GetByIdAsync(dto.InvoiceId);
        }
    }
}
