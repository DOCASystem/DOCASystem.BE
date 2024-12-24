using AutoMapper;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Response.Animal;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;

namespace DOCA.API.Mappers;

public class AnimalMapper : Profile
{
    public AnimalMapper()
    {
        CreateMap<CreateAnimalRequest, Animal>();
        CreateMap<Animal, GetAnimalDetailResponse>()
            .ForMember(dest => dest.AnimalCategories, 
                opt => opt.MapFrom(src => src.AnimalCategoryRelationship!.Select(pc => pc.AnimalCategory)))
            .ForMember(dest => dest.AnimalImage, 
                opt => opt.MapFrom(src => src.AnimalImage));
        CreateMap<Animal, GetAnimalResponse>()
            .ForMember(dest => dest.AnimalImage, 
                opt => opt.MapFrom(src => src.AnimalImage));
    }
}