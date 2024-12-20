using AutoMapper;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class ProductMapper : Profile
{
    public ProductMapper()
    {
        CreateMap<Product, GetProductDetailResponse>()
            .ForMember(dest => dest.Categories, 
                opt => opt.MapFrom(src => src.ProductCategories!.Select(pc => pc.Category)))
            .ForMember(dest => dest.ProductImages, 
                opt => opt.MapFrom(src => src.ProductImages));
        CreateMap<Product, GetProductResponse>()
            .ForMember(dest => dest.ProductImages, 
                opt => opt.MapFrom(src => src.ProductImages));
    }
}