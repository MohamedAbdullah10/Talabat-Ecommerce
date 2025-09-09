using LinkDev.Talabat.Core.Domain.Contract.Infrastructure;
using LinkDev.Talabat.Infrastructure._BasketRepository;
using LinkDev.Talabat.Infrastructure.CacheService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;



namespace LinkDev.Talabat.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton(typeof(IConnectionMultiplexer), (_) =>
            {
             var connectionstring= configuration.GetConnectionString("Redis");
                var multiplexer = ConnectionMultiplexer.Connect(connectionstring!);
                return multiplexer;


            });

            services.AddScoped<IBasketRepository, BasketRepository>();
            services.AddScoped<IPaymentService, PaymentService.PaymentService>();
            services.AddSingleton(typeof(IResponseCacheService), typeof(ResponseCacheService));

            return services;
        }
    }
}
