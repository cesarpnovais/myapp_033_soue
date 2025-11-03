using Microsoft.AspNetCore.Mvc;
using PayFlow.Api.Models;
using PayFlow.Api.Services;

namespace PayFlow.Api.Controllers
{
    [ApiController]
    [Route("payments")]
    public class PaymentsController : ControllerBase
    {
        private readonly PaymentService _paymentService;

        public PaymentsController(PaymentService paymentService)
        {
            _paymentService = paymentService;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] PaymentRequest request)
        {
            if (request == null || request.Amount <= 0)
                return BadRequest(new { error = "Invalid request" });

            var result = await _paymentService.ProcessAsync(request);
            return Ok(result);
        }
    }
}
