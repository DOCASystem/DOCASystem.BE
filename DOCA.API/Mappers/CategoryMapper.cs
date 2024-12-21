using AutoMapper;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class CategoryMapper : Profile
{
    public CategoryMapper()
    {
        CreateMap<CreateCategoryRequest, Category>();
        CreateMap<Category, CategoryResponse>();
    }
}