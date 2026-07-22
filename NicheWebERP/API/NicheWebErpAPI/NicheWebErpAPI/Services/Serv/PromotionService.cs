using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Models;
using NicheWebErpAPI.Repository.IRepo;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Services.Serv
{
    public class PromotionService : IPromotionService
    {
        private readonly IPromotionRepository _repository;
        private readonly ICurrentUserService _currentUserService;

        public PromotionService(IPromotionRepository repository, ICurrentUserService currentUserService)
        {
            _repository = repository;
            _currentUserService = currentUserService;
        }

        public Task<List<PromotionListItemDto>> GetAllAsync() => _repository.GetAllAsync(_currentUserService.CompanyId);

        public async Task<PromotionListItemDto> CreateAsync(CreatePromotionDto dto)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;

            var promotion = await _repository.CreateAsync(companyId, dto, updatedById);

            return new PromotionListItemDto
            {
                Id = promotion.EntityID,
                Description = promotion.Description,
                IsCoupon = promotion.IsCoupon,
                CouponCode = promotion.CouponCode,
                CouponDiscountPrintedValue = promotion.CouponDiscountPrintedValue,
                CouponIsDollar = promotion.CouponIsDollar,
                CouponDiscountMinimumSpend = promotion.CouponDiscountMinimumSpend,
                CouponMaxUses = promotion.CouponMaxUses,
                CouponCurrentUses = promotion.CouponCurrentUses,
                CouponMaxUsesPerson = promotion.CouponMaxUsesPerson,
                StartDate = promotion.StartDate,
                EndDate = promotion.EndDate
            };
        }

        // Never mutates anything - safe to call for live UI feedback as well as from
        // SalesOrderService.CreateAsync just before actually applying the discount.
        public async Task<ValidateCouponResultDto> ValidateCouponAsync(string couponCode, Guid firmId, decimal orderSubTotal)
        {
            var companyId = _currentUserService.CompanyId;

            var promotion = await _repository.GetByCouponCodeAsync(companyId, couponCode);
            if (promotion is null)
            {
                return new ValidateCouponResultDto { Valid = false, Message = "Coupon code not found." };
            }

            var now = DateTime.UtcNow;
            if (now < promotion.StartDate)
            {
                return new ValidateCouponResultDto { Valid = false, Message = "This coupon isn't active yet." };
            }
            if (promotion.EndDate.HasValue && now > promotion.EndDate.Value)
            {
                return new ValidateCouponResultDto { Valid = false, Message = "This coupon has expired." };
            }
            if (orderSubTotal < promotion.CouponDiscountMinimumSpend)
            {
                return new ValidateCouponResultDto
                {
                    Valid = false,
                    Message = $"Order must be at least {promotion.CouponDiscountMinimumSpend:C} to use this coupon."
                };
            }
            // 0 = unlimited, same convention as Firm.CreditLimit (Sprint 05).
            if (promotion.CouponMaxUses > 0 && promotion.CouponCurrentUses >= promotion.CouponMaxUses)
            {
                return new ValidateCouponResultDto { Valid = false, Message = "This coupon has reached its usage limit." };
            }

            var firm = await _repository.GetFirmAsync(companyId, firmId);
            if (firm is not null && firm.MainContactID.HasValue && firm.MainContactID.Value != Guid.Empty
                && promotion.CouponMaxUsesPerson > 0)
            {
                var couponPerson = await _repository.GetCouponPersonAsync(companyId, promotion.EntityID, firm.MainContactID.Value);
                if ((couponPerson?.Uses ?? 0) >= promotion.CouponMaxUsesPerson)
                {
                    return new ValidateCouponResultDto { Valid = false, Message = "This customer has already used this coupon the maximum number of times." };
                }
            }

            var discountAmount = promotion.CouponIsDollar
                ? promotion.CouponDiscountPrintedValue
                : orderSubTotal * promotion.CouponDiscountPrintedValue;

            return new ValidateCouponResultDto { Valid = true, DiscountAmount = discountAmount, PromotionId = promotion.EntityID };
        }

        // Only called once an order is actually created with the coupon applied - never from the
        // standalone ValidateCoupon endpoint. Deliberately does NOT call SaveChangesAsync itself -
        // the caller (SalesOrderService.CreateAsync) adds this to the same tracked-but-unsaved
        // context as the rest of the order and saves everything in one transaction at the end.
        public async Task RecordUsageAsync(Guid promotionId, Guid firmId, decimal discountAmount, decimal orderSubTotal)
        {
            var companyId = _currentUserService.CompanyId;
            var updatedById = _currentUserService.IsAuthenticated ? _currentUserService.LegacyPersonId : Guid.Empty;
            var now = DateTime.UtcNow;

            var promotion = await _repository.GetByIdAsync(companyId, promotionId);
            if (promotion is null)
            {
                return;
            }

            promotion.CouponCurrentUses += 1;
            promotion.CouponCurrentValueLocalCurrency += discountAmount;
            promotion.TotalOrderValueLocalCurrency += orderSubTotal;
            promotion.LastUpdated = now;
            promotion.UpdatedByID = updatedById;
            _repository.UpdatePromotion(promotion);

            var firm = await _repository.GetFirmAsync(companyId, firmId);
            if (firm is not null && firm.MainContactID.HasValue && firm.MainContactID.Value != Guid.Empty)
            {
                var personId = firm.MainContactID.Value;
                var couponPerson = await _repository.GetCouponPersonAsync(companyId, promotionId, personId);
                if (couponPerson is null)
                {
                    couponPerson = new CouponPerson
                    {
                        CompanyID = companyId,
                        EntityID = Guid.NewGuid(),
                        PromotionID = promotionId,
                        PersonID = personId,
                        Uses = 1,
                        ValueDiscountLocalCurrency = discountAmount,
                        ValueOrderLocalCurrency = orderSubTotal,
                        LastUpdated = now,
                        UpdatedByID = updatedById
                    };
                    await _repository.AddCouponPersonAsync(couponPerson);
                }
                else
                {
                    couponPerson.Uses += 1;
                    couponPerson.ValueDiscountLocalCurrency += discountAmount;
                    couponPerson.ValueOrderLocalCurrency += orderSubTotal;
                    couponPerson.LastUpdated = now;
                    couponPerson.UpdatedByID = updatedById;
                    _repository.UpdateCouponPerson(couponPerson);
                }
            }
        }
    }
}
