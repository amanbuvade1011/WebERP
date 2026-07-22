using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PromotionsController : ControllerBase
    {
        private readonly IPromotionService _promotionService;

        public PromotionsController(IPromotionService promotionService)
        {
            _promotionService = promotionService;
        }

        // GET api/Promotions/GetAllPromotions
        [HttpGet("GetAllPromotions")]
        public async Task<ActionResult<IEnumerable<PromotionListItemDto>>> GetAllPromotions() =>
            Ok(await _promotionService.GetAllAsync());

        // POST api/Promotions/CreatePromotion
        [HttpPost("CreatePromotion")]
        public async Task<ActionResult<PromotionListItemDto>> CreatePromotion(CreatePromotionDto dto) =>
            Ok(await _promotionService.CreateAsync(dto));

        // POST api/Promotions/ValidateCoupon
        [HttpPost("ValidateCoupon")]
        public async Task<ActionResult<ValidateCouponResultDto>> ValidateCoupon(ValidateCouponRequestDto dto) =>
            Ok(await _promotionService.ValidateCouponAsync(dto.CouponCode, dto.FirmId, dto.OrderSubTotal));
    }
}
