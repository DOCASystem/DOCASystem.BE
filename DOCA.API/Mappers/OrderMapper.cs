using AutoMapper;
using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.Order;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product))
            .ForMember(dest => dest.Blog, opt => opt.MapFrom(src => src.Blog));

        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.Member, opt => opt.MapFrom(src => src.Member));
    }
}