using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TradingTermsController : ControllerBase
    {
        private readonly ITradingTermsService _termsService;

        public TradingTermsController(ITradingTermsService termsService)
        {
            _termsService = termsService;
        }

        // GET api/TradingTerms/GetAllTerms
        [HttpGet("GetAllTerms")]
        public async Task<ActionResult<IEnumerable<TradingTermsDto>>> GetAllTerms() =>
            Ok(await _termsService.GetAllAsync());
    }
}
