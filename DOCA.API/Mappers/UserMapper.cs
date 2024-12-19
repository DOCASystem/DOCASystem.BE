using AutoMapper;
using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Response.Account;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class UserMapper : Profile
{
    public UserMapper()
    {
        CreateMap<SignUpRequest, User>();
        CreateMap<User, LoginResponse>();
    }
}