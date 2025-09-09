using LinkDev.Talabat.APIs.Controllers.Controllers.Base;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using LinkDev.Talabat.Core.Application.Abstraction.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.APIs.Controllers.Controllers.Order
{
    [Authorize]
    [Route("api/orders")]
    public class OrderController(IServiceManager manager) :BaseApiController
    {
        [HttpPost]
        public async Task<ActionResult<OrderToRetunDto>> CreateOrder(OrderToCreateDto order) {

            var buyeremail = User.FindFirstValue(ClaimTypes.Email);
            var result= await manager.OrderService.CreateOrderAsync(buyeremail!, order);
            return Ok(result);



        }
        [HttpGet]
        public async Task<ActionResult<IEnumerable<OrderToRetunDto>>> GetOrdersForUser() {
            var buyeremail = User.FindFirstValue(ClaimTypes.Email);
            var result = await manager.OrderService.GetOrdersForUserAsync(buyeremail!);
            return Ok(result);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToRetunDto>> GetOrderByIdForUser(int id) {
            var buyeremail = User.FindFirstValue(ClaimTypes.Email);
            var result = await manager.OrderService.GetOrderByIdAsync(buyeremail!, id);
            if (result is null)
                return NotFound();
            return Ok(result);
        }
        [HttpGet("deliveryMethods")]
        public async Task<ActionResult<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethods() {
            var result = await manager.OrderService.GetDeliveryMethodsAsync();
            return Ok(result);
        }
    }
}
