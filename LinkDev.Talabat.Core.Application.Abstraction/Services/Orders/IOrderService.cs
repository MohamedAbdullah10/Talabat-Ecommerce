using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Application.Abstraction.Services.Orders
{
    public interface IOrderService
    {
        Task<OrderToRetunDto> CreateOrderAsync(string buyerEmail,OrderToCreateDto order);
        Task<IEnumerable<OrderToRetunDto>> GetOrdersForUserAsync(string buyerEmail);

        Task<OrderToRetunDto?> GetOrderByIdAsync(string buyerEmail, int orderId);

        Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync();
    }
}
