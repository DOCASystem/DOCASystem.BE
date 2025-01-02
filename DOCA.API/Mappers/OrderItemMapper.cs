using AutoMapper;
using DOCA.API.Payload.Response.Order;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class OrderItemMapper : Profile
{
    public OrderItemMapper()
    {
        CreateMap<OrderItem, OrderItemResponse>()
            .ForMember(dest => dest.Product, opt => opt.MapFrom(src => src.Product));
    }
}