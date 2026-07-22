namespace NicheWebErpAPI.Dtos
{
    public class PromotionListItemDto
    {
        public Guid Id { get; set; }
        public string? Description { get; set; }
        public bool IsCoupon { get; set; }
        public string? CouponCode { get; set; }
        public decimal CouponDiscountPrintedValue { get; set; }
        public bool CouponIsDollar { get; set; }
        public decimal CouponDiscountMinimumSpend { get; set; }
        public int CouponMaxUses { get; set; }
        public int CouponCurrentUses { get; set; }
        public int CouponMaxUsesPerson { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
    }

    public class CreatePromotionDto
    {
        public string Description { get; set; } = null!;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public bool IsCoupon { get; set; }
        public string? CouponCode { get; set; }
        public decimal CouponDiscountPrintedValue { get; set; }
        public bool CouponIsDollar { get; set; }
        public decimal CouponDiscountMinimumSpend { get; set; }
        public int CouponMaxUses { get; set; }
        public int CouponMaxUsesPerson { get; set; }
    }

    public class ValidateCouponRequestDto
    {
        public string CouponCode { get; set; } = null!;
        public Guid FirmId { get; set; }
        public decimal OrderSubTotal { get; set; }
    }

    public class ValidateCouponResultDto
    {
        public bool Valid { get; set; }
        public string? Message { get; set; }
        public decimal DiscountAmount { get; set; }
        public Guid? PromotionId { get; set; }
    }
}
