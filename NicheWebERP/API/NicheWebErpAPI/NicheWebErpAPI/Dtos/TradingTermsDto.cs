namespace NicheWebErpAPI.Dtos
{
    public class TradingTermsDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public int NumberOfDays { get; set; }
        public int DiscountDays { get; set; }
        public decimal SettlementDiscountPercent { get; set; }
    }
}
