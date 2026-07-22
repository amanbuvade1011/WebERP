namespace NicheWebErpAPI.Models
{
    // Maps to dbo.Promotion - a promotion/campaign, optionally a coupon (IsCoupon + CouponCode).
    // 13 live rows, all MarketType1=2 with no lookup table to decode it (confirmed 2026-07-21) -
    // left unenforced, see docs/ai-plan/sprints/sprint-07-promotions-freight.md. DiscountRule (the
    // other discount mechanism referenced from TransactionDiscount) has 0 live rows and no
    // rule-definition columns at all - dead end, not used here. Only the columns this sprint's
    // CreatePromotion/ValidateCoupon actually use are meaningfully populated on create; the
    // remaining ~20 web-storefront display columns (BigPictureWidth, MetaTagTitle, etc.) get
    // defaults since this admin tool doesn't manage storefront presentation.
    public class Promotion
    {
        public Guid CompanyID { get; set; }
        public Guid EntityID { get; set; }

        public string? Description { get; set; }
        public string? LongDescription { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public Guid SeasonID { get; set; }
        public Guid CategoryID { get; set; }
        public Guid LabelID { get; set; }
        public bool ValidWithOtherOffers { get; set; }
        public string? ManualUrl { get; set; }
        public bool ManualStyleSelection { get; set; }
        public bool ForHomePage { get; set; }
        public int BigPictureWidth { get; set; }
        public int BigPictureHeight { get; set; }
        public int SmallPictureWidth { get; set; }
        public int SmallPictureHeight { get; set; }
        public Guid RangeID { get; set; }
        public bool InvitationOnly { get; set; }
        public int MarketType1 { get; set; }
        public int Sequence { get; set; }
        public int Container { get; set; }
        public string? MetaTagTitle { get; set; }
        public string? MetaTagDescription { get; set; }
        public string? WebSiteID { get; set; }
        public bool AllStyles { get; set; }

        public bool IsCoupon { get; set; }
        public string? CouponCode { get; set; }
        public decimal CouponDiscountPrintedValue { get; set; }
        public decimal CouponDiscountMinimumSpend { get; set; }
        public bool CouponIsDollar { get; set; }
        public int CouponMaxUses { get; set; }
        public decimal CouponMaxValueLocalCurrency { get; set; }
        public int CouponCurrentUses { get; set; }
        public decimal CouponCurrentValueLocalCurrency { get; set; }
        public int CouponMaxUsesPerson { get; set; }
        public decimal TotalOrderValueLocalCurrency { get; set; }

        public DateTime? LastUpdated { get; set; }
        public Guid? UpdatedByID { get; set; }
    }
}
