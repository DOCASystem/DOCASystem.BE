using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Response.Account;

namespace DOCA.API.Services.Interface;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);

    Task<LoginResponse> RegisterAsync(SignUpRequest request);
}