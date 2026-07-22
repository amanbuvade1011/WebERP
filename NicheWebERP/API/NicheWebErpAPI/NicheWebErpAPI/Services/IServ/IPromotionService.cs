using NicheWebErpAPI.Dtos;

namespace NicheWebErpAPI.Services.IServ
{
    public interface IPromotionService
    {
        Task<List<PromotionListItemDto>> GetAllAsync();
        Task<PromotionListItemDto> CreateAsync(CreatePromotionDto dto);
        Task<ValidateCouponResultDto> ValidateCouponAsync(string couponCode, Guid firmId, decimal orderSubTotal);
        Task RecordUsageAsync(Guid promotionId, Guid firmId, decimal discountAmount, decimal orderSubTotal);
    }
}
