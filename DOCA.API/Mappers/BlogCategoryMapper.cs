using AutoMapper;
using DOCA.API.Payload.BlogCategory;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class BlogCategoryMapper: Profile
{
    public BlogCategoryMapper()
    {
        CreateMap<CreateBlogCategoryRequest, BlogCategory>();
        CreateMap<BlogCategory, BlogCategoryResponse>();
    }
}