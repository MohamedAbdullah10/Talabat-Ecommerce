using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Basket;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Orders;
using LinkDev.Talabat.Core.Application.Exceptions;
using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using LinkDev.Talabat.Core.Domain.Entities.Orders;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Core.Domain.Specifications.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Application.Services.Orders
{
    internal class OrderService(IUnitOfWork unitOfWork,IBasketService basketService,IMapper mapper,IPaymentService paymentService) : IOrderService
    {
        public async Task<OrderToRetunDto> CreateOrderAsync(string buyerEmail, OrderToCreateDto order)
        {
            // 1.Get Basket From Baskets Repo

            var basket = await basketService.GetCustomerBasketAsync(order.BasketId);

            // 2. Get Selected Items at Basket From Products Repo

            var orderItems = new List<OrderItem>();

            if (basket.Items.Count > 0)
            {
                var productRepo = unitOfWork.GetRepository<Product, int>();

                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);

                    if (product is not null)
                    {
                        var productItemOrdered = new ProductItemOrdered(product.Id, product.Name, product.PictureUrl);

                        var orderItem = new OrderItem(productItemOrdered, product.Price, item.Quantity);

                        orderItems.Add(orderItem);
                    }
                }
            }

            // 3. Calculate SubTotal

            var orderSubtotal = orderItems.Sum(item => item.Price * item.Quantity);

            // 4. Mapping

            var orderAddress = mapper.Map<Address>(order.ShippingAddress);
            // 4.1 Get Delivery Method
            var deliveryMethod = await unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(order.DeliveryMethodId);

            // 5. Create Order
            var orderRepo = unitOfWork.GetRepository<Order, int>();
            var orderspec = new OrderWithPaymentIntentSpecification(basket.PaymentIntentId);
            var existingOrder = await orderRepo.GetByIdWithSpecAsync(orderspec);
            if (existingOrder is not null)
            {
                orderRepo.Delete(existingOrder);
                await paymentService.CreateOrUpdatePaymentIntent(basket.Id);
                // Re-fetch basket to get the updated PaymentIntentId after refresh
                basket = await basketService.GetCustomerBasketAsync(order.BasketId);
            }

            // Ensure we have a valid PaymentIntentId on the basket before creating the order
            if (string.IsNullOrEmpty(basket.PaymentIntentId))
            {
                await paymentService.CreateOrUpdatePaymentIntent(basket.Id);
                basket = await basketService.GetCustomerBasketAsync(order.BasketId);
            }

            var createdOrder = new Order()
            {
                BuyerEmail = buyerEmail,
                ShippingAddress = orderAddress,
                Items = orderItems,
                Subtotal = orderSubtotal,
                PaymentIntentId=basket.PaymentIntentId!,
                DeliveryMethod=deliveryMethod!
                
            };

            await orderRepo.AddAsync(createdOrder);


            // 6. Save To Database 

            var created = await unitOfWork.CompleteAsync() > 0;

            if (!created) throw new BadRequestException("an error has been occuered when creating the order");

            return mapper.Map<OrderToRetunDto>(createdOrder);

        }
        public async Task<OrderToRetunDto?> GetOrderByIdAsync(string buyerEmail, int orderId)
        {
            var orderspec= new OrderSpecifications(buyerEmail, orderId);
            var order = await unitOfWork.GetRepository<Order, int>().GetByIdWithSpecAsync(orderspec);
            if (order == null) throw new NotFoundException(nameof(Order), "notfound");
            return mapper.Map<OrderToRetunDto>(order);
        }

        public async Task<IEnumerable<OrderToRetunDto>> GetOrdersForUserAsync(string buyerEmail)
        {
            var orderspec = new OrderSpecifications(buyerEmail);
            var orders = await unitOfWork.GetRepository<Order, int>().GetAllWithSpecAsync(orderspec);
            if (orders == null) throw new NotFoundException(nameof(Order), "notfound"); 
            return mapper.Map<IEnumerable<OrderToRetunDto>>(orders);
        }
        public async Task<IEnumerable<DeliveryMethodDto>> GetDeliveryMethodsAsync()
        {
           var del= await unitOfWork.GetRepository<DeliveryMethod, int>().GetAllAsync();
            return mapper.Map<IEnumerable<DeliveryMethodDto>>(del);
        }

        
    }
}
