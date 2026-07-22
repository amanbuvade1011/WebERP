namespace NicheWebErpAPI.Dtos
{
    public class RecordPaymentDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Narration { get; set; }

        // Added Sprint 10 - optional so existing callers (and any pre-Sprint-10 test data) keep
        // working; a payment recorded without one just doesn't count toward any cashbook balance.
        public Guid? CashbookId { get; set; }
    }
}
