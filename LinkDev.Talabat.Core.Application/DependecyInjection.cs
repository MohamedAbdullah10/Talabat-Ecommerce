using LinkDev.Talabat.Core.Application.Abstraction.Services;
using LinkDev.Talabat.Core.Application.Mapping;
using LinkDev.Talabat.Core.Application.Services;
using Microsoft.Extensions.DependencyInjection;
using AutoMapper;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Basket;
using LinkDev.Talabat.Core.Application.Services.Basket;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Auth;
using LinkDev.Talabat.Core.Application.Services.Auth;
using LinkDev.Talabat.Core.Application.Abstraction.Services.Orders;
using LinkDev.Talabat.Core.Application.Services.Orders;

namespace LinkDev.Talabat.Core.Application
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            //services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>());
            services.AddAutoMapper(typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IServiceManager),typeof(ServiceManager));
            services.AddScoped(typeof(IBasketService), typeof(BasketService));
            services.AddScoped(typeof(Func<IBasketService>), (serviceporvider) =>
            {

                return () => serviceporvider.GetRequiredService<IBasketService>();

            });
            services.AddScoped(typeof(IOrderService), typeof(OrderService));
            services.AddScoped(typeof(Func<IOrderService>), (serviceporvider) =>
            {
                return () => serviceporvider.GetRequiredService<IOrderService>();
            });
            return services;
        }
    }
}
