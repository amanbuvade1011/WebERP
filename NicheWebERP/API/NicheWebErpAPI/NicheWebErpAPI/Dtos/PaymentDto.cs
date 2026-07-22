namespace NicheWebErpAPI.Dtos
{
    public class RecordPaymentDto
    {
        public Guid InvoiceId { get; set; }
        public decimal Amount { get; set; }
        public Guid PaymentMethodId { get; set; }
        public string? Narration { get; set; }
    }
}
