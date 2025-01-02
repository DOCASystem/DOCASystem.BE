using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Payload.Response.Cart;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class CartMapper : Profile
{
    public CartMapper()
    {
        CreateMap<Product, CartModelResponse>()
            .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));
        
        CreateMap<Blog, CartModelResponse>()
            .ForMember(dest => dest.BlogId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.BlogName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.BlogDescription, opt => opt.MapFrom(src => src.Description));
    }
}