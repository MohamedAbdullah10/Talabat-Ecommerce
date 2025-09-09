using LinkDev.Talabat.Core.Domain.Entities.Orders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Specifications.Orders
{
    public class OrderSpecifications :BaseSpecifications<Order,int>
    {
        public OrderSpecifications(string buyerEmail, int orderId):base(order=>
        order.BuyerEmail== buyerEmail && order.Id==orderId)
        {
           Includes.Add(o=>o.DeliveryMethod);
              Includes.Add(o=>o.Items);

        }
        public OrderSpecifications(string buyerEmail):base(order =>order.BuyerEmail == buyerEmail)
        {
            Includes.Add(o => o.DeliveryMethod);
            Includes.Add(o => o.Items);
        }
    }
}
