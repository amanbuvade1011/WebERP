namespace NicheWebErpAPI.Models
{
    // Maps to dbo.TradingTerms - payment terms (net days, settlement discount).
    public class TradingTerms
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public int NumberOfDays { get; set; }
        public int DaysFrom1 { get; set; }
        public int DiscountDays { get; set; }
        public string? Description { get; set; }
        public decimal SettlementDiscountPercent { get; set; }

        public DateTime LastUpdated { get; set; }
        public Guid UpdatedByID { get; set; }
    }
}
