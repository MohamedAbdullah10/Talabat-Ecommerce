using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Basket;
using LinkDev.Talabat.Core.Application.Exceptions;
using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Core.Domain.Entities.Basket;

using Microsoft.Extensions.Configuration;
using Microsoft.VisualBasic;

namespace LinkDev.Talabat.Core.Application.Services.Basket
{
    internal class BasketService(IBasketRepository basketRepository, IMapper mapper, IConfiguration configuration) : IBasketService
    {
     
        public async Task<CustomerBasketDto> GetCustomerBasketAsync(string basketId)
        {
           var basket= await basketRepository.GetBasket(basketId);
            if (basket == null) {

                throw new NotFoundException(nameof(CustomerBasket), basketId);
            }
           return mapper.Map<CustomerBasketDto>(basket);

        }

        public async Task<CustomerBasketDto> UpdateCustomerBasketAsync(CustomerBasketDto basketDto)
        {
            var basket = mapper.Map<CustomerBasket>(basketDto);
            var timeToLive = TimeSpan.FromDays(double.Parse(configuration.GetSection("RedisSettings")["TimeToLiveInDays"]!));
            var update = await basketRepository.UpdateBasket(basket, timeToLive);
            if (update is null) throw new BadRequestException("can't update, there is a problem with this basket.");
            return basketDto;
        }
        public async Task DeleteCustomerBasketAsync(string basketId)
        {
            var basket = await basketRepository.DeleteBasket(basketId);
            if (!basket) throw new BadRequestException("unable to delete this basket.");

        }
    }
}
