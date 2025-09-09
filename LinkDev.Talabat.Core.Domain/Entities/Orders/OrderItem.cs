using LinkDev.Talabat.Core.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Entities.Orders
{
    public class OrderItem : BaseAuditableEntity<int>
    {
        public OrderItem()
        {
            
        }
        

        public OrderItem(ProductItemOrdered itemOrdered, decimal price, int quantity)
        {
            Product = itemOrdered;
            Price = price;
            Quantity = quantity;
        }

        public virtual ProductItemOrdered Product { get; set; } = null!;

        public decimal Price { get; set; }
        public int Quantity { get; set; }
    }
}
