using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Response.Order;
using DOCA.API.Services.Interface;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class OrderService : BaseService<OrderService>, IOrderService
{
    private IConfiguration _configuration;
    public OrderService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<OrderService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<OrderResponse>> GetOrderList(int page, int size, OrderFilter? filter, string? sortBy, bool isAsc)
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        var role = GetRoleFromJwt();
        
        IPaginate<OrderResponse> orderResponses;
        switch (role)
        {
            case RoleEnum.Member:
                var member = await _unitOfWork.GetRepository<Member>().SingleOrDefaultAsync(
                    predicate: m => m.UserId == userId,
                    include: m => m.Include(m => m.User)
                );
                if(member == null) throw new BadHttpRequestException(MessageConstant.User.UserNotFound);
                var ordersWithMemberId = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                    selector: o => new Order()
                    {
                        Id = o.Id,
                        CreatedAt = o.CreatedAt,
                        ModifiedAt = o.ModifiedAt,
                        Status = o.Status,
                        Total = o.Total,
                        Address = o.Address,
                        MemberId = o.MemberId,
                        PaymentId = o.PaymentId,
                        Member = o.Member,
                    },
                    predicate: o => o.MemberId == member.Id,
                    page: page,
                    size: size,
                    filter: filter,
                    sortBy: sortBy,
                    isAsc: isAsc,
                    include: o => o.Include(o => o.Member)
                        .ThenInclude(m => m.User)
                        .Include(o => o.Payment)
                );
                orderResponses = _mapper.Map<IPaginate<OrderResponse>>(ordersWithMemberId);
                break;
            case RoleEnum.Manager:
                var allorders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                    selector: o => new Order()
                    {
                        Id = o.Id,
                        CreatedAt = o.CreatedAt,
                        ModifiedAt = o.ModifiedAt,
                        Status = o.Status,
                        Total = o.Total,
                        Address = o.Address,
                        MemberId = o.MemberId,
                        PaymentId = o.PaymentId,
                        Member = o.Member
                    },
                    page: page,
                    size: size,
                    filter: filter,
                    sortBy: sortBy,
                    isAsc: isAsc,
                    include: o => o.Include(o => o.Member)
                        .ThenInclude(m => m.User)
                        .Include(o => o.Payment)
                );
                orderResponses = _mapper.Map<IPaginate<OrderResponse>>(allorders);
                break;
            case RoleEnum.Staff:
                var orders = await _unitOfWork.GetRepository<Order>().GetPagingListAsync(
                    selector: o => new Order()
                    {
                        Id = o.Id,
                        CreatedAt = o.CreatedAt,
                        ModifiedAt = o.ModifiedAt,
                        Status = o.Status,
                        Total = o.Total,
                        Address = o.Address,
                        MemberId = o.MemberId,
                        PaymentId = o.PaymentId,
                        Member = o.Member
                    },
                    page: page,
                    size: size,
                    filter: filter,
                    sortBy: sortBy,
                    isAsc: isAsc,
                    include: o => o.Include(o => o.Member)
                        .ThenInclude(m => m.User)
                        .Include(o => o.Payment)
                );
                orderResponses = _mapper.Map<IPaginate<OrderResponse>>(orders);
                break;
            default:
                throw new BadHttpRequestException(MessageConstant.User.RoleNotFound);
        }
        return orderResponses;
    }

    public async Task<ICollection<OrderItemResponse>> GetOrderItemsByOrderId(Guid orderId)
    {
        if (orderId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Order.OrderIdNotNull);

        var orderItems = await _unitOfWork.GetRepository<OrderItem>().GetListAsync(
            predicate: oi => oi.OrderId == orderId,
            include: oi => oi.Include(oi => oi.Product)
                .ThenInclude(p => p.ProductImages)
                .Include(oi => oi.Blog) 
        );

        var response = _mapper.Map<ICollection<OrderItemResponse>>(orderItems);
        return response;
    }

}