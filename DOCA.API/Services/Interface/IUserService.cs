using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Request.Member;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Member;

namespace DOCA.API.Services.Interface;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RegisterAsync(SignUpRequest request);

   Task<MemberResponse> GetMemberInformationAsync();

   Task<UserResponse> UpdateMemberAsync(UpdateMemberRequest request);
}