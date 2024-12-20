using System.Security.Claims;
using AutoMapper;
using DOCA.API.Enums;
using DOCA.Domain.Models;
using DOCA.Repository.Interfaces;

namespace DOCA.API.Services;

public class BaseService<T> where T : class
{
    protected IUnitOfWork<DOCADbContext> _unitOfWork;
    protected ILogger<T> _logger;
    protected IMapper _mapper;
    protected IHttpContextAccessor _httpContextAccessor;
    
    public BaseService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<T> logger, IMapper mapper,
        IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
        _mapper = mapper;
        _httpContextAccessor = httpContextAccessor;
    }
    protected RoleEnum GetRoleFromJwt()
    {
        string roleString = _httpContextAccessor?.HttpContext?.User.FindFirstValue(ClaimTypes.Role);
        if (string.IsNullOrEmpty(roleString)) return RoleEnum.None;
        
        Enum.TryParse<RoleEnum>(roleString, out RoleEnum role);
        return role;
        
    }

    protected Guid GetUserIdFromJwt()
    {
        var userIdClaim = _httpContextAccessor.HttpContext?.User?.FindFirst("userId");
        if (userIdClaim != null)
        {
            return Guid.Parse(userIdClaim.Value);
        }
        return Guid.Empty;
    }
}