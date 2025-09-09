using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Core.Domain.Entities.Basket;
using StackExchange.Redis;
using System.Text.Json;

namespace LinkDev.Talabat.Infrastructure._BasketRepository
{
    public class BasketRepository : IBasketRepository
    {

        private readonly IDatabase database;
        public BasketRepository(IConnectionMultiplexer connection)
        {
            database = connection.GetDatabase();
        }


        public async Task<CustomerBasket?> GetBasket(string id)
        {
            var data = await database.StringGetAsync(id);
            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<CustomerBasket>(data!);

        }

        public async Task<CustomerBasket?> UpdateBasket(CustomerBasket basket, TimeSpan timeToLive)
        {
            var updaedData = await database.StringSetAsync(basket.Id, JsonSerializer.Serialize(basket), timeToLive);
            if (!updaedData) return null;
            return basket;
            
        }
        public Task<bool> DeleteBasket(string id)
        {
            return database.KeyDeleteAsync(id); 
        }

     
    }
}
