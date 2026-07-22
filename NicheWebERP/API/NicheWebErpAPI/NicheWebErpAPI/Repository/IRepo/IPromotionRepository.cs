using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;

namespace NicheWebErpAPI.Repository.IRepo
{
    public interface IPromotionRepository
    {
        Task<List<PromotionListItemDto>> GetAllAsync(Guid companyId);
        Task<Promotion> CreateAsync(Guid companyId, CreatePromotionDto dto, Guid updatedById);

        Task<Promotion?> GetByCouponCodeAsync(Guid companyId, string couponCode);
        Task<Promotion?> GetByIdAsync(Guid companyId, Guid id);
        Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId);
        Task<CouponPerson?> GetCouponPersonAsync(Guid companyId, Guid promotionId, Guid personId);

        void UpdatePromotion(Promotion promotion);
        Task AddCouponPersonAsync(CouponPerson couponPerson);
        void UpdateCouponPerson(CouponPerson couponPerson);

        Task<int> SaveChangesAsync();
    }
}
