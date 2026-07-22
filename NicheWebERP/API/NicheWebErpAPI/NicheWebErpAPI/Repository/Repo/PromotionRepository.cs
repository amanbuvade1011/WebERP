using Microsoft.EntityFrameworkCore;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;

namespace NicheWebErpAPI.Repository.Repo
{
    public class PromotionRepository : IPromotionRepository
    {
        private readonly IEfCoreService<Promotion> _promotionService;
        private readonly IEfCoreService<CouponPerson> _couponPersonService;
        private readonly IEfCoreService<Firm> _firmService;

        public PromotionRepository(
            IEfCoreService<Promotion> promotionService,
            IEfCoreService<CouponPerson> couponPersonService,
            IEfCoreService<Firm> firmService)
        {
            _promotionService = promotionService;
            _couponPersonService = couponPersonService;
            _firmService = firmService;
        }

        public Task<List<PromotionListItemDto>> GetAllAsync(Guid companyId) =>
            _promotionService.Query()
                .Where(p => p.CompanyID == companyId)
                .OrderByDescending(p => p.StartDate)
                .Select(p => new PromotionListItemDto
                {
                    Id = p.EntityID,
                    Description = p.Description,
                    IsCoupon = p.IsCoupon,
                    CouponCode = p.CouponCode,
                    CouponDiscountPrintedValue = p.CouponDiscountPrintedValue,
                    CouponIsDollar = p.CouponIsDollar,
                    CouponDiscountMinimumSpend = p.CouponDiscountMinimumSpend,
                    CouponMaxUses = p.CouponMaxUses,
                    CouponCurrentUses = p.CouponCurrentUses,
                    CouponMaxUsesPerson = p.CouponMaxUsesPerson,
                    StartDate = p.StartDate,
                    EndDate = p.EndDate
                })
                .ToListAsync();

        public async Task<Promotion> CreateAsync(Guid companyId, CreatePromotionDto dto, Guid updatedById)
        {
            var now = DateTime.UtcNow;
            var promotion = new Promotion
            {
                CompanyID = companyId,
                EntityID = Guid.NewGuid(),
                Description = dto.Description,
                StartDate = dto.StartDate,
                EndDate = dto.EndDate,
                // Web-storefront display/scoping columns this admin tool doesn't manage - see
                // Models/Promotion.cs. MarketType1 left at 0 (unknown/unspecified) rather than
                // copying the unexplained "2" every live row uses - see sprint doc.
                SeasonID = Guid.Empty,
                CategoryID = Guid.Empty,
                LabelID = Guid.Empty,
                RangeID = Guid.Empty,
                MarketType1 = 0,
                ValidWithOtherOffers = true,
                ManualStyleSelection = false,
                ForHomePage = false,
                BigPictureWidth = 0,
                BigPictureHeight = 0,
                SmallPictureWidth = 0,
                SmallPictureHeight = 0,
                InvitationOnly = false,
                Sequence = 0,
                Container = 0,
                AllStyles = true,
                IsCoupon = dto.IsCoupon,
                CouponCode = dto.CouponCode,
                CouponDiscountPrintedValue = dto.CouponDiscountPrintedValue,
                CouponDiscountMinimumSpend = dto.CouponDiscountMinimumSpend,
                CouponIsDollar = dto.CouponIsDollar,
                CouponMaxUses = dto.CouponMaxUses,
                CouponMaxValueLocalCurrency = 0,
                CouponCurrentUses = 0,
                CouponCurrentValueLocalCurrency = 0,
                CouponMaxUsesPerson = dto.CouponMaxUsesPerson,
                TotalOrderValueLocalCurrency = 0,
                LastUpdated = now,
                UpdatedByID = updatedById
            };

            await _promotionService.AddAsync(promotion);
            await _promotionService.SaveChangesAsync();
            return promotion;
        }

        public Task<Promotion?> GetByCouponCodeAsync(Guid companyId, string couponCode) =>
            _promotionService.Query().FirstOrDefaultAsync(
                p => p.CompanyID == companyId && p.IsCoupon && p.CouponCode == couponCode);

        public Task<Promotion?> GetByIdAsync(Guid companyId, Guid id) =>
            _promotionService.Query().FirstOrDefaultAsync(p => p.CompanyID == companyId && p.EntityID == id);

        public Task<Firm?> GetFirmAsync(Guid companyId, Guid firmId) =>
            _firmService.Query().FirstOrDefaultAsync(f => f.CompanyID == companyId && f.EntityID == firmId);

        public Task<CouponPerson?> GetCouponPersonAsync(Guid companyId, Guid promotionId, Guid personId) =>
            _couponPersonService.Query().FirstOrDefaultAsync(
                cp => cp.CompanyID == companyId && cp.PromotionID == promotionId && cp.PersonID == personId);

        public void UpdatePromotion(Promotion promotion) => _promotionService.Update(promotion);
        public Task AddCouponPersonAsync(CouponPerson couponPerson) => _couponPersonService.AddAsync(couponPerson);
        public void UpdateCouponPerson(CouponPerson couponPerson) => _couponPersonService.Update(couponPerson);

        public Task<int> SaveChangesAsync() => _promotionService.SaveChangesAsync();
    }
}
