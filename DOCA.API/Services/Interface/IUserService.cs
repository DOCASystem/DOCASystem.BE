using System.Reflection;
using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Request.Member;
using DOCA.API.Payload.Request.Staff;
using DOCA.API.Payload.Request.User;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Member;
using DOCA.API.Payload.Response.Staff;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using MemberFilter = DOCA.Domain.Filter.MemberFilter;

namespace DOCA.API.Services.Interface;

public interface IUserService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
    Task<LoginResponse> RegisterAsync(SignUpRequest request);
   Task<MemberResponse> GetMemberInformationAsync();
   Task<UserResponse> UpdateMemberAsync(UpdateMemberRequest request);
   Task<string> GenerateOtpAsync(GenerateEmailOtpRequest request);
   Task<UserResponse> ForgetPassword(ForgetPasswordRequest request);
   Task<IPaginate<MemberResponse>> GetMembersAsync(int page, int size, MemberFilter? filter, string? sortBy, bool isAsc);
   Task<IPaginate<StaffResponse>> GetStaffsAsync(int page, int size, StaffFilter? filter, string? sortBy, bool isAsc);
   Task<UserResponse> UpdateStaffAsync(Guid staffId, UpdateStaffRequest request);
   Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request);
    
   Task<StaffResponse> GetStaffById(Guid id);
}