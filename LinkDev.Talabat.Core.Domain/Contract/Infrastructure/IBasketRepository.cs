using LinkDev.Talabat.Core.Domain.Entities.Basket;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkDev.Talabat.Core.Domain.Contract.Infrastructure
{
    public interface IBasketRepository
    {
        Task<CustomerBasket?> GetBasket(string id);
        Task<CustomerBasket?> UpdateBasket(CustomerBasket basket,TimeSpan timeSpan);
        Task<bool> DeleteBasket(string id);
    }
}
