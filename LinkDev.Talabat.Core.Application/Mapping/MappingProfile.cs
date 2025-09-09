using AutoMapper;

using LinkDev.Talabat.Core.Application.Abstraction.Models.Orders;
using LinkDev.Talabat.Core.Application.Abstraction.Models.Products;
using LinkDev.Talabat.Core.Domain.Entities.Basket;
using LinkDev.Talabat.Core.Domain.Entities.Orders;
using LinkDev.Talabat.Core.Domain.Entities.Products;
using LinkDev.Talabat.Core.Application.Abstraction;

namespace LinkDev.Talabat.Core.Application.Mapping
{
    public class MappingProfile :Profile
    {
        public MappingProfile()
        {
            CreateMap<Product, ProductToReturnDto>().ForMember(dest => dest.Brand, opt => opt.MapFrom(src => src.Brand!.Name))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category!.Name)).ForMember(dest=>dest.PictureUrl,opt=>opt.MapFrom<ProductPictureUrlResolver>());
            CreateMap<ProductBrand, BrandDto>();
            CreateMap<ProductCategory, CategoryDto>();

            CreateMap<BasketItem,BasketItemDto>().ReverseMap();
            CreateMap<CustomerBasket,CustomerBasketDto>().ReverseMap();


            CreateMap<AddressDto, Address>().ReverseMap();

            // Mapping بين Order و OrderToRetunDto
            CreateMap<Order, OrderToRetunDto>()
                .ForMember(d => d.ShippingAddress, config => config.MapFrom(s => s.ShippingAddress))
                .ForMember(d => d.DeliveryMethod, config => config.MapFrom(s => s.DeliveryMethod.ShortName));

            // Mapping بين OrderItem و OrderItemDto
            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(d => d.ProductId, config => config.MapFrom(s => s.Product.ProductId))
                .ForMember(d => d.ProductName, config => config.MapFrom(s => s.Product.ProductName))
                .ForMember(d => d.PictureUrl, config => config.MapFrom(s => s.Product.PictureUrl));

            CreateMap<DeliveryMethod, DeliveryMethodDto>()
    .ForMember(dest => dest.ShortName, opt => opt.MapFrom(src => src.ShortName))
    .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description));







            CreateMap<Domain.Entities.Identity.Address, AddressDto>().ReverseMap();



        }
    }
}
