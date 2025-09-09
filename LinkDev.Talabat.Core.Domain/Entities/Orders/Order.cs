using LinkDev.Talabat.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Entities.Orders
{
    public class Order : BaseAuditableEntity<int>
    {
        public Order()
        {

        }
        public Order(string buyerEmail, Address shippingAddress, int deliveryMethodId, DeliveryMethod deliveryMethod, ICollection<OrderItem> items, decimal subtotal)
        {
            BuyerEmail = buyerEmail;
            ShippingAddress = shippingAddress;
            DeliveryMethodId = deliveryMethodId;
            DeliveryMethod = deliveryMethod;
            Items = items;
            Subtotal = subtotal;
        }
        public required string BuyerEmail { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public virtual required Address ShippingAddress { get; set; }

        public int? DeliveryMethodId { get; set; }
        public virtual DeliveryMethod DeliveryMethod { get; set; }

        public virtual ICollection<OrderItem> Items { get; set; } = new HashSet<OrderItem>();

        public decimal Subtotal { get; set; }

        public decimal GetTotal() => Subtotal + (DeliveryMethod?.Cost?? 0);

        public string PaymentIntentId { get; set; } = string.Empty;
    }
}
