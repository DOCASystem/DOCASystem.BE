using AutoMapper;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Member;
using DOCA.Domain.Models;

namespace DOCA.API.Mappers;

public class MemberMapper : Profile
{
    public MemberMapper()
    {
        CreateMap<Member, MemberResponse>();
        CreateMap<User, UserResponse>();
    }
}