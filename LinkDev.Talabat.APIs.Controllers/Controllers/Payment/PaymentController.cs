using LinkDev.Talabat.APIs.Controllers.Controllers.Base;
using LinkDev.Talabat.APIs.Controllers.Errors;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Core.Domain.Entities.Basket;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Payment
{
  
    [Route("api/payments")]
    public class PaymentController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;

        //// This is your Stripe CLI webhook secret for testing your endpoint locally.
        //private const string whSecret = "whsec_da03d1e17f93edb229ecf6c6419f017ff0cc58a9fe05ea605db466a00be4cd5b";

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }


        //[ProducesResponseType(typeof(CustomerBasket), StatusCodes.Status200OK)]
        //[ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
        [Authorize]
        [HttpPost("{basketid}")] // GET : /api/payments/{basketid}
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket is null) return BadRequest(new ApiResponse(400, "An Error with your Basket"));

            return Ok(basket);
        }
        [AllowAnonymous]
        [HttpPost("webhook")]
        public async Task<IActionResult> WebHook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            await _paymentService.UpdateOrderPaymentStatus(json, Request.Headers["Stripe-Signature"]!);
            return Ok();
        }
    }
}
