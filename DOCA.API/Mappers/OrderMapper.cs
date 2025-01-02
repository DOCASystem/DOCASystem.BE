using AutoMapper;
using DOCA.API.Payload.Response.Order;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class OrderMapper : Profile
{
    public OrderMapper()
    {
        CreateMap<Order, OrderResponse>()
            .ForMember(dest => dest.Member, 
                opt => opt.MapFrom(src => src.Member));
    }
}