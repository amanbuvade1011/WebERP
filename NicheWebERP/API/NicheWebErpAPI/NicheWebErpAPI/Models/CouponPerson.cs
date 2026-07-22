namespace NicheWebErpAPI.Models
{
    // Maps to dbo.CouponPerson - per-person coupon usage/value tracking. Person-keyed (retail),
    // but our order flow is Firm-keyed (wholesale) - bridged via Firm.MainContactID when set. See
    // docs/ai-plan/sprints/sprint-07-promotions-freight.md.
    public class CouponPerson
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public Guid PromotionID { get; set; }
        public Guid PersonID { get; set; }
        public int Uses { get; set; }
        public decimal ValueDiscountLocalCurrency { get; set; }
        public decimal ValueOrderLocalCurrency { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
