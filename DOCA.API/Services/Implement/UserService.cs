using System.Linq.Expressions;
using System.Reflection;
using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Account;
using DOCA.API.Payload.Request.Member;
using DOCA.API.Payload.Request.Staff;
using DOCA.API.Payload.Request.User;
using DOCA.API.Payload.Response.Account;
using DOCA.API.Payload.Response.Member;
using DOCA.API.Payload.Response.Staff;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using StackExchange.Redis;
using JsonSerializer = System.Text.Json.JsonSerializer;
using MemberFilter = DOCA.Domain.Filter.MemberFilter;

namespace DOCA.API.Services.Implement;

public class UserService : BaseService<UserService>, IUserService
{
    private IConfiguration _configuration;
    private readonly IRedisService _redisService;
    public UserService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<UserService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IRedisService redisService) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _redisService = redisService;
    }

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        Expression<Func<User, bool>> searchFilter = p =>
            p.Username.Equals(request.UsernameOrPhoneNumber) &&
            p.Password.Equals(PasswordUtil.HashPassword(request.Password));
        var account = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(predicate: searchFilter);
        if (account == null) throw new UnauthorizedAccessException(MessageConstant.User.LoginFail);
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
        var userList = await _unitOfWork.GetRepository<User>().GetListAsync();

        if (userList.Any(u => u.Username == request.Username))
            throw new BadHttpRequestException(MessageConstant.User.UserNameExisted);

        if (userList.Any(u => u.PhoneNumber == request.PhoneNumber))
            throw new BadHttpRequestException(MessageConstant.User.PhoneNumberExisted);

        var user = _mapper.Map<User>(request);

        // Kiểm tra OTP qua Redis Service
        var key = request.Username;
        var existingOtp = await _redisService.GetStringAsync(key);

        if (string.IsNullOrEmpty(existingOtp))
            throw new BadHttpRequestException(MessageConstant.Otp.OtpNotFound);
        
        if (existingOtp != request.Otp)
            throw new BadHttpRequestException(MessageConstant.Otp.OtpIncorrect);

        user.Password = PasswordUtil.HashPassword(request.Password);
        user.Role = RoleEnum.Member;

        var member = new Member { Id = Guid.NewGuid(), UserId = user.Id, User = user };

        // Sử dụng TransactionScope để quản lý giao dịch
        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                await _unitOfWork.GetRepository<User>().InsertAsync(user);

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
    
    public async Task<MemberResponse> GetMemberInformationAsync()
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        
        var member = await _unitOfWork.GetRepository<Member>().SingleOrDefaultAsync(
            predicate: o => o.UserId.Equals(userId),
            include: m => m.Include(x => x.User) 
            );
        if (member == null) throw new BadHttpRequestException(MessageConstant.User.MemberNotFound);
        
        var response = _mapper.Map<MemberResponse>(member);
        response.Username = member.User.Username;
        response.FullName = member.User.FullName;
        response.PhoneNumber = member.User.PhoneNumber;
        return response;
    }
    
    public async Task<UserResponse> UpdateMemberAsync(UpdateMemberRequest request)
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        var member = await _unitOfWork.GetRepository<Member>().SingleOrDefaultAsync(
            predicate: m => m.UserId == userId,
            include: m => m.Include(m => m.User)
        );
        if (member == null) throw new BadHttpRequestException(MessageConstant.User.MemberNotFound);
        request.TrimString();
        member.Commune = string.IsNullOrEmpty(request.Commune) ? member.Commune : request.Commune;
        member.Province = string.IsNullOrEmpty(request.Province) ? member.Province : request.Province;
        member.District = string.IsNullOrEmpty(request.District) ? member.District : request.District;
        member.Address = string.IsNullOrEmpty(request.Address) ? member.Address : request.Address;
        member.ProvinceCode = request.ProvinceCode ?? member.ProvinceCode;
        member.DistrictCode = request.DistrictCode ?? member.DistrictCode;
        member.CommuneCode = request.CommuneCode ?? member.CommuneCode;
        member.User.Username = string.IsNullOrEmpty(request.Username) ? member.User.Username : request.Username;
        member.User.FullName = string.IsNullOrEmpty(request.FullName) ? member.User.FullName : request.FullName;

         _unitOfWork.GetRepository<Member>().UpdateAsync(member);
         var isSuccess = await _unitOfWork.CommitAsync() > 0;
        UserResponse response = null;
        if (isSuccess) response = _mapper.Map<UserResponse>(member.User);
        return response;
    }


    // public async Task<string> GenerateOtpAsync(GenerateOtpRequest request)
    // {
    //     // var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
    //     // var db = redis.GetDatabase();
    //     var key = request.PhoneNumber;
    //     
    //     var existingOtp = await _redisService.GetStringAsync(key);
    //     if (!string.IsNullOrEmpty(existingOtp)) throw new BadHttpRequestException(MessageConstant.Sms.OtpAlreadySent);
    //     
    //     if(request.PhoneNumber == null) throw new BadHttpRequestException(MessageConstant.User.PhoneNumberNotFound);
    //     var phoneNumberArray = new string[] { request.PhoneNumber };
    //     var otp = OtpUtil.GenerateOtp();
    //     var content = "Mã OTP của bạn là: " + otp;
    //     var response = SmsUtil.sendSMS(phoneNumberArray, content, _configuration);
    //     _logger.LogInformation(response);
    //     var smsResponse = JsonSerializer.Deserialize<SmsModel.SmsResponse>(response);
    //     if (smsResponse.status != "success" && smsResponse.code != "00")
    //     {
    //         throw new BadHttpRequestException(MessageConstant.Sms.SendSmsFailed);
    //     }
    //     
    //     await _redisService.SetStringAsync(key, otp, TimeSpan.FromMinutes(2));
    //     return request.PhoneNumber;
    // }
    
    
    public async Task<string> GenerateOtpAsync(GenerateEmailOtpRequest request)
    {
        if (request == null || string.IsNullOrEmpty(request.Email))
            throw new BadHttpRequestException(MessageConstant.Otp.EmailRequired);

        if (_redisService == null)
            throw new InvalidOperationException(MessageConstant.Redis.RedisServiceNotInitialized);

        var key = request.Email;

        var existingOtp = await _redisService.GetStringAsync(key);
        if (!string.IsNullOrEmpty(existingOtp))
            throw new BadHttpRequestException(MessageConstant.Otp.OtpAlreadySent);

        var otp = OtpUtil.GenerateOtp();
        var subject = "Mã OTP của bạn";
        var body = $"Mã OTP của bạn là: <b>{otp}</b>. Mã này có hiệu lực trong 2 phút.";

        var response = SmsUtil.SendEmail(request.Email, subject, body, _configuration);
        _logger.LogInformation($"📧 Đã gửi email OTP: {response}");

        if (!response)
        {
            _logger.LogError($" {MessageConstant.Email.SendEmailFailed}");
            throw new BadHttpRequestException(MessageConstant.Otp.SendOtpFailed);
        }

        try
        {
            await _redisService.SetStringAsync(key, otp, TimeSpan.FromMinutes(2));
            _logger.LogInformation($" OTP [{otp}] đã được lưu vào Redis cho email {request.Email}");
            return otp;
        }
        catch (Exception ex)
        {
            _logger.LogError($" {MessageConstant.Otp.SaveOtpFailed}: {ex.Message}");
            throw new BadHttpRequestException(MessageConstant.Otp.SendOtpFailed);
        }
    }





    public async Task<UserResponse> ForgetPassword(ForgetPasswordRequest request)
    {
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: u => u.PhoneNumber == request.PhoneNumber
        );
        var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
        var db = redis.GetDatabase();
        var key = request.PhoneNumber;
        var existingOtp = await _redisService.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(existingOtp)) throw new BadHttpRequestException(MessageConstant.Otp.OtpNotFound);
        if (existingOtp != request.Otp) throw new BadHttpRequestException(MessageConstant.Otp.OtpIncorrect);
        
        user.Password = PasswordUtil.HashPassword(request.Password);
        _unitOfWork.GetRepository<User>().UpdateAsync(user);
        UserResponse response = null;
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if (isSuccess) response = _mapper.Map<UserResponse>(user);
        return response;
    }

    public async Task<IPaginate<MemberResponse>> GetMembersAsync(int page, int size, MemberFilter? filter, string? sortBy, bool isAsc)
    {
        var members = await _unitOfWork.GetRepository<Member>().GetPagingListAsync(
            selector: m => new Member()
            {
                Id = m.Id,
                UserId = m.UserId,
                User = m.User,
                Address = m.Address,
                Commune = m.Commune,
                District = m.District,
                Province = m.Province,
                ProvinceCode = m.ProvinceCode,
                CommuneCode = m.CommuneCode,
                DistrictCode = m.DistrictCode,
                Orders = m.Orders
            },
            page: page,
            size: size,
            filter: filter,
            include: m => m.Include(m => m.User),
            sortBy: sortBy,
            isAsc: isAsc
        );
        var response = _mapper.Map<IPaginate<MemberResponse>>(members);
        return response;
    }

    public async Task<IPaginate<StaffResponse>> GetStaffsAsync(int page, int size, StaffFilter? filter, string? sortBy, bool isAsc)
    {
        var staffs = await _unitOfWork.GetRepository<Staff>().GetPagingListAsync(
            selector: s => new Staff()
            {
                Id = s.Id,
                Type = s.Type,
                User = s.User,
                UserId = s.UserId
            },
            page: page,
            size: size,
            filter: filter,
            include: s => s.Include(s => s.User),
            sortBy: sortBy,
            isAsc: isAsc
        );
        var response = _mapper.Map<IPaginate<StaffResponse>>(staffs);
        return response;
    }

    public async Task<UserResponse> UpdateStaffAsync(Guid staffId, UpdateStaffRequest request)
    {
        if (staffId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.StaffIdNotNull);
        var staff = await _unitOfWork.GetRepository<Staff>().SingleOrDefaultAsync(
            predicate: s => s.Id == staffId,
            include: s => s.Include(s => s.User)
            );
        if (staff == null) throw new BadHttpRequestException(MessageConstant.User.StaffNotFound);
        request.TrimString();
        staff.Type = (StaffType)(!request.Type.HasValue ? staff.Type : request.Type);
        staff.User.Username = string.IsNullOrEmpty(request.Username) ? staff.User.Username : request.Username;
        staff.User.Password = string.IsNullOrEmpty(request.Password) ? staff.User.Password : PasswordUtil.HashPassword(request.Password);
        staff.User.FullName = string.IsNullOrEmpty(request.FullName) ? staff.User.FullName : request.FullName;
        
        _unitOfWork.GetRepository<Staff>().UpdateAsync(staff);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        UserResponse response = null;
        if (isSuccess) response = _mapper.Map<UserResponse>(staff.User);
        return response;
    }

    public async Task<StaffResponse> CreateStaffAsync(CreateStaffRequest request)
    {
        var userList = await _unitOfWork.GetRepository<User>().GetListAsync();
        if (userList.Any(u => u.Username == request.Username)) throw new BadHttpRequestException(MessageConstant.User.UserNameExisted);
        if (userList.Any(u => u.PhoneNumber == request.PhoneNumber)) throw new BadHttpRequestException(MessageConstant.User.PhoneNumberExisted);
        var user = _mapper.Map<User>(request);
        user.Password = PasswordUtil.HashPassword(request.Password);
        user.Role = RoleEnum.Staff;
        var staff = new Staff()
        {
            Id = Guid.NewGuid(),
            Type = request.Type,
            UserId = user.Id,
            User = user
        };
        await _unitOfWork.GetRepository<Staff>().InsertAsync(staff);
        StaffResponse response = null;
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        if(isSuccess) return _mapper.Map<StaffResponse>(staff);
        return response;
    }

    public async Task<StaffResponse> GetStaffById(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.User.StaffIdNotNull);
        var staff = await _unitOfWork.GetRepository<Staff>().SingleOrDefaultAsync(
            predicate: s => s.Id == id,
            include: s => s.Include(s => s.User)
            );
        if (staff == null) throw new BadHttpRequestException(MessageConstant.User.StaffNotFound);
        var response = _mapper.Map<StaffResponse>(staff);
        return response;
    }
}