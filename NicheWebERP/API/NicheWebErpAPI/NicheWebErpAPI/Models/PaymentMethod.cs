namespace NicheWebErpAPI.Models
{
    // Maps to dbo.PaymentMethod - named payment method (Cash, EFTPOS, Credit Card...) with
    // channel-allow flags and surcharge. 4 live rows, confirmed 2026-07-20.
    public class PaymentMethod
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string? Description { get; set; }
        public int PaymentMethodType1 { get; set; }
        public bool AllowWholesale { get; set; }
        public bool AllowRetail { get; set; }
        public bool AllowPOS { get; set; }
        public decimal SurchargePercentage { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
