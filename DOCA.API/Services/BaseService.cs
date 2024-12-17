using AutoMapper;
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
}