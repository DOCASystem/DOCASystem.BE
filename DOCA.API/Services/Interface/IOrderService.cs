using DOCA.API.Payload.Response.Order;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IOrderService
{
    Task<IPaginate<OrderResponse>> GetOrderList(int page, int size, OrderFilter? filter, string? sortBy, bool isAsc);
    Task<ICollection<OrderItemResponse>> GetOrderItemsByOrderId(Guid orderId);
}