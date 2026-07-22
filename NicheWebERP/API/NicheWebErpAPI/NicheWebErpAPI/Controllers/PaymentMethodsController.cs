using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NicheWebErpAPI.Dtos;
using NicheWebErpAPI.Services.IServ;

namespace NicheWebErpAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class PaymentMethodsController : ControllerBase
    {
        private readonly IPaymentMethodService _paymentMethodService;

        public PaymentMethodsController(IPaymentMethodService paymentMethodService)
        {
            _paymentMethodService = paymentMethodService;
        }

        // GET api/PaymentMethods/GetAllPaymentMethods
        [HttpGet("GetAllPaymentMethods")]
        public async Task<ActionResult<IEnumerable<PaymentMethodDto>>> GetAllPaymentMethods() =>
            Ok(await _paymentMethodService.GetAllAsync());
    }
}
