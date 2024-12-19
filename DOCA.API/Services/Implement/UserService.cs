using System.Linq.Expressions;
using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Models;
using DOCA.Repository.Interfaces;

namespace DOCA.API.Services.Implement;

public class UserService : BaseService<UserService>, IUserService
{
    private IConfiguration _configuration;
    public UserService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        Expression<Func<User, bool>> searchFilter = p =>
            p.Username.Equals(request.UsernameOrPhoneNumber) &&
            p.Password.Equals(PasswordUtil.HashPassword(request.Password));
        var account = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: searchFilter);
        if (account == null) throw new BadHttpRequestException(MessageConstant.User.UserNotFound);
        RoleEnum role = EnumUtil.ParseEnum<RoleEnum>(account.Role.GetDescriptionFromEnum());
        Tuple<string, Guid> guidClaim = null;
        LoginResponse response = null;
        switch (role)
        {
            case RoleEnum.Member:
                guidClaim = new Tuple<string, Guid>("userId", account.Id);
                response = new LoginResponse()
                {
                    Id = account.Id,
                    Username = account.Username,
                    PhoneNumber = account.PhoneNumber,
                    FullName = account.FullName,
                };
                break;
            case RoleEnum.Staff:
                guidClaim = new Tuple<string, Guid>("userID", account.Id);
                response = new LoginResponse()
                {
                    Id = account.Id,
                    Username = account.Username,
                    PhoneNumber = account.PhoneNumber,
                    FullName = account.FullName,
                };
                break;
            case RoleEnum.Manager:
                guidClaim = new Tuple<string, Guid>("userID", account.Id);
                response = new LoginResponse()
                {
                    Id = account.Id,
                    Username = account.Username,
                    PhoneNumber = account.PhoneNumber,
                    FullName = account.FullName,
                };
                break;
            default:
                response = new LoginResponse();
                break;
        }
        
        var token = JwtUtil.GenerateJwtToken(account, guidClaim, _configuration);
        var refreshToken = JwtUtil.GenerateRefreshToken();

        response.Token = token;
        response.RefreshToken = refreshToken;
        
        _unitOfWork.GetRepository<User>().UpdateAsync(account);
        var isSuccessFully = await _unitOfWork.CommitAsync() > 0;
        if (isSuccessFully)
        {
            return response;
        }

        return null;
    }
    
    public async Task<LoginResponse> RegisterAsync(SignUpRequest request)
{
    // Lấy danh sách user từ UnitOfWork
    var userRepository = _unitOfWork.GetRepository<User>();
    var userList = await userRepository.GetListAsync();
    
    if (userList.Any(u => u.Username == request.Username))
        throw new BadHttpRequestException(MessageConstant.User.UserNameExisted);
    
    if (userList.Any(u => u.PhoneNumber == request.PhoneNumber))
        throw new BadHttpRequestException(MessageConstant.User.PhoneNumberExisted);
    
    // Mapping request sang đối tượng User
    var user = _mapper.Map<User>(request);

    // Kiểm tra OTP qua Redis Service
    // var key = request.PhoneNumber;
    // var existingOtp = await _redisService.GetStringAsync(key);

    // if (string.IsNullOrEmpty(existingOtp))
    //     throw new BadHttpRequestException(MessageConstant.Sms.OtpNotFound);
    //
    // if (existingOtp != request.Otp)
    //     throw new BadHttpRequestException(MessageConstant.Sms.OtpIncorrect);

    user.Password = PasswordUtil.HashPassword(request.Password);
    user.Role = RoleEnum.Member.ToString();

    var member = new Member
    {
        Id = Guid.NewGuid(),
        UserId = user.Id,
        User = user
    };

    // Sử dụng TransactionScope để quản lý giao dịch
    using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
    {
        try
        {
            await userRepository.InsertAsync(user);

            var memberRepository = _unitOfWork.GetRepository<Member>();
            await memberRepository.InsertAsync(member);

            bool isSuccessful = await _unitOfWork.CommitAsync() > 0;

            transaction.Complete();

            if (isSuccessful)
            {
                var response = _mapper.Map<LoginResponse>(user);
                response.Token = JwtUtil.GenerateJwtToken(user, new Tuple<string, Guid>("userId", user.Id),
                    _configuration);
                response.RefreshToken = JwtUtil.GenerateRefreshToken();
                return response;
            }

            return null;
        }
        catch (TransactionException ex)
        {
            _logger.LogError(ex.Message);
            return null;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            return null;
        }
    }
}

}