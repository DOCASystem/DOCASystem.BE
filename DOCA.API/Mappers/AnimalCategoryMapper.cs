using AutoMapper;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Response.Animal;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class AnimalCategoryMapper : Profile
{
    public AnimalCategoryMapper()
    {
        CreateMap<CreateAnimalCategoryRequest, AnimalCategory>();
        CreateMap<AnimalCategory, AnimalCategoryResponse>();
    }
}