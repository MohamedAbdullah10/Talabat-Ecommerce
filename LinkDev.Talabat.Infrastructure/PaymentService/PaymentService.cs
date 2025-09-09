using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Application.Exceptions;
using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Core.Domain.Contract.Persistence;
using LinkDev.Talabat.Core.Domain.Entities.Basket;
using LinkDev.Talabat.Core.Domain.Entities.Orders;
using LinkDev.Talabat.Core.Domain.Specifications.Orders;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Product = LinkDev.Talabat.Core.Domain.Entities.Products.Product;

namespace LinkDev.Talabat.Infrastructure.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PaymentService> _logger;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public PaymentService(
            IConfiguration configuration,
            ILogger<PaymentService> logger,
            IBasketRepository basketRepo,
            IUnitOfWork unitOfWork,
            IMapper mapper)
        {
            _logger = logger;
            _configuration = configuration;
            _basketRepo = basketRepo;
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }
        public async Task<CustomerBasket?> CreateOrUpdatePaymentIntent(string basketId)
        {
            StripeConfiguration.ApiKey = _configuration["StripeSettings:SecretKey"];

            var basket = await _basketRepo.GetBasket(basketId);
            if (basket is null) return null;

            var shippingPrice = 0m;

            if (basket.DeliveryMethodId.HasValue)
            {
                var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(basket.DeliveryMethodId.Value);
                shippingPrice = deliveryMethod!.Cost;
                basket.ShippingPrice = shippingPrice;
            }

            if (basket.Items?.Count > 0)
            {
                var productRepo = _unitOfWork.GetRepository<Product, int>();
                foreach (var item in basket.Items)
                {
                    var product = await productRepo.GetByIdAsync(item.Id);
                    if (item.Price != product!.Price)
                        item.Price = product.Price;
                }
            }

            PaymentIntent paymentIntent;
            PaymentIntentService paymentIntentService = new PaymentIntentService();

            if (string.IsNullOrEmpty(basket.PaymentIntentId)) // Create New Payment Intent
            {
                var options = new PaymentIntentCreateOptions()
                {
                    Amount = (long)basket!.Items!.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                    Currency = "usd",
                    PaymentMethodTypes = new List<string>() { "card" }
                };

                paymentIntent = await paymentIntentService.CreateAsync(options); // Integration with Stripe

                basket.PaymentIntentId = paymentIntent.Id;
                basket.ClientSecret = paymentIntent.ClientSecret;

            }
            else // Update Exisiting Payment Intent
            {
                var options = new PaymentIntentUpdateOptions()
                {
                    Amount = (long)basket!.Items!.Sum(item => item.Price * 100 * item.Quantity) + (long)shippingPrice * 100,
                };

                await paymentIntentService.UpdateAsync(basket.PaymentIntentId, options);

            }

            await _basketRepo.UpdateBasket(basket, TimeSpan.FromMinutes(3));


            return basket;
        }

        public async Task<Order?> UpdateOrderPaymentStatus(string request, string header)
        {
            var whSecret = _configuration.GetRequiredSection("StripeSettings")["whSecret"];
            try
            {
                _logger.LogInformation("Stripe webhook received. Signature: {0}", header);
                var stripeEvent = EventUtility.ConstructEvent(request, header, whSecret);
                _logger.LogInformation("Stripe event type: {0}", stripeEvent.Type);

                Order? order = null;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.succeeded":
                        {
                            if (stripeEvent.Data.Object is PaymentIntent succeededIntent)
                            {
                                _logger.LogInformation("Processing payment_intent.succeeded for PaymentIntentId: {0}", succeededIntent.Id);
                                order = await UpdatePaymentReceived(succeededIntent.Id);
                                _logger.LogInformation("Order update result: {0}, PaymentIntentId: {1}", order != null ? "Success" : "Failed", order?.PaymentIntentId);
                            }
                            else
                            {
                                _logger.LogWarning("Received payment_intent.succeeded with unexpected payload type: {0}", stripeEvent.Data.Object?.GetType().FullName);
                            }
                            break;
                        }
                    case "payment_intent.payment_failed":
                        {
                            if (stripeEvent.Data.Object is PaymentIntent failedIntent)
                            {
                                order = await UpdatePaymentFailed(failedIntent.Id);
                                _logger.LogInformation("Order is Failed {0}", order?.PaymentIntentId);
                            }
                            else
                            {
                                _logger.LogWarning("Received payment_intent.payment_failed with unexpected payload type: {0}", stripeEvent.Data.Object?.GetType().FullName);
                            }
                            break;
                        }
                    default:
                        // Ignore other event types (e.g., charge.*) to avoid casting errors
                        _logger.LogInformation("Ignoring non-payment_intent event type: {0}", stripeEvent.Type);
                        break;
                }

                return order;
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe webhook signature verification failed or event error");
                return null; // Do not rethrow to avoid 500 to Stripe
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled error processing Stripe webhook");
                return null; // Do not rethrow to avoid 500 to Stripe
            }
        }

        public async Task<Order?> UpdateOrderPaymentStatus(string paymentIntentId, bool isPaid)
        {
            var orderRepo = _unitOfWork.GetRepository<Order, int>();

            var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);

            var order = await orderRepo.GetByIdWithSpecAsync(spec);

            if (order is null) return null;

            if (isPaid)
                order.Status = OrderStatus.PaymentReceived;
            else
                order.Status = OrderStatus.PaymentFailed;

            order.LastModifiedBy = "Manual Update"; // Set LastModifiedBy to avoid NULL constraint violation
            order.LastModifiedOn = DateTime.UtcNow;

            orderRepo.Update(order);

            await _unitOfWork.CompleteAsync();

            return order;
        }



        private async Task<Order?> UpdatePaymentReceived(string paymentIntentId)
        {
            _logger.LogInformation("Attempting to update order status for PaymentIntentId: {0}", paymentIntentId);

            // Debug: List all orders to see what's in the database
            var allOrders = await _unitOfWork.GetRepository<Order, int>().GetAllAsync();
            _logger.LogInformation("Total orders in database: {0}", allOrders.Count());
            foreach (var o in allOrders.Take(5)) // Show first 5 orders
            {
                _logger.LogInformation("Order {0}: PaymentIntentId='{1}', Status={2}", o.Id, o.PaymentIntentId, o.Status);
            }

            // Retry mechanism in case order is not yet created
            for (int attempt = 1; attempt <= 3; attempt++)
            {
                _logger.LogInformation("Attempt {0}: Searching for order with PaymentIntentId: '{1}'", attempt, paymentIntentId);
                var spec = new OrderWithPaymentIntentSpecification(paymentIntentId);
                var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithSpecAsync(spec);
                if (order is not null)
                {
                    _logger.LogInformation("Found order {0} with current status {1}, updating to PaymentReceived", order.Id, order.Status);



                    order.Status = OrderStatus.PaymentReceived;
                    order.LastModifiedBy = "Stripe Webhook"; // Set LastModifiedBy to avoid NULL constraint violation
                    order.LastModifiedOn = DateTime.UtcNow;

                    _unitOfWork.GetRepository<Order, int>().Update(order);
                    var result = await _unitOfWork.CompleteAsync();

                    _logger.LogInformation("Order {0} status updated successfully. CompleteAsync result: {1}", order.Id, result);
                    return order;
                }

                _logger.LogWarning("Attempt {0}: No order found for PaymentIntentId {1}, waiting 2 seconds before retry", attempt, paymentIntentId);

                // Alternative search method - direct database query
                var allOrdersDirect = await _unitOfWork.GetRepository<Order, int>().GetAllAsync();
                var matchingOrder = allOrdersDirect.FirstOrDefault(o => o.PaymentIntentId == paymentIntentId);
                if (matchingOrder != null)
                {
                    _logger.LogInformation("Found order via direct search: Order {0}, Status: {1}", matchingOrder.Id, matchingOrder.Status);
                    matchingOrder.Status = OrderStatus.PaymentReceived;
                    matchingOrder.LastModifiedBy = "Stripe Webhook"; // Set LastModifiedBy to avoid NULL constraint violation
                    matchingOrder.LastModifiedOn = DateTime.UtcNow;

                    _unitOfWork.GetRepository<Order, int>().Update(matchingOrder);
                    var result = await _unitOfWork.CompleteAsync();
                    _logger.LogInformation("Order {0} status updated via direct search. CompleteAsync result: {1}", matchingOrder.Id, result);
                    return matchingOrder;
                }

                if (attempt < 3)
                {
                    await Task.Delay(2000); // Wait 2 seconds before retry
                }
            }

            _logger.LogError("Failed to find order for PaymentIntentId {0} after 3 attempts", paymentIntentId);
            return null;
        }

        private async Task<Order?> UpdatePaymentFailed(string paymentIntentId)
        {
            var order = await _unitOfWork.GetRepository<Order, int>().GetByIdWithSpecAsync(new OrderWithPaymentIntentSpecification(paymentIntentId));
            if (order is null)
            {
                _logger.LogWarning("No order found for PaymentIntentId {0} when handling payment_intent.payment_failed", paymentIntentId);
                return null;
            }

            order.Status = OrderStatus.PaymentFailed;
            order.LastModifiedBy = "Stripe Webhook"; // Set LastModifiedBy to avoid NULL constraint violation
            order.LastModifiedOn = DateTime.UtcNow;

            _unitOfWork.GetRepository<Order, int>().Update(order);
            await _unitOfWork.CompleteAsync();

            return order;
        }

        // Explicit interface implementation removed; public method above satisfies the interface
    }
}
