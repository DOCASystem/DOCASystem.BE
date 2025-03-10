using AutoMapper;
using DOCA.API.Payload.Request.Blog;
using DOCA.API.Payload.Response.Blog;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class BlogMapper : Profile
{
    public BlogMapper()
    {
        CreateMap<CreateBlogRequest, Blog>();
        CreateMap<Blog, GetBlogDetailResponse>()
            .ForMember(dest => dest.BlogCategories,
                opt => opt.MapFrom(src => src.BlogCategoryRelationship!.Select(pc => pc.BlogCategory)))
            .ForMember(dest => dest.Animals,
            opt => opt.MapFrom(src => src.BlogAnimal!.Select(pc => pc.Animal)));
        CreateMap<Blog, GetBlogResponse>();
    }
}