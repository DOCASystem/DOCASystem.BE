using AutoMapper;
using DOCA.Domain.Paginate;

namespace DOCA.API.Mappers;

public class PaginateMapper : Profile
{
    public PaginateMapper()
    {
        CreateMap(typeof(IPaginate<>), typeof(IPaginate<>)).ConvertUsing(typeof(PaginateConverter<,>));
    }
        
}