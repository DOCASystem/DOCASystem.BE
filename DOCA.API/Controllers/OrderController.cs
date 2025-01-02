using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Response.Order;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Order.OrderEndpoint)]
public class OrderController : BaseController<OrderController>
{
    private readonly IOrderService _orderService;
    public OrderController(ILogger<OrderController> logger, IOrderService orderService) : base(logger)
    {
        _orderService = orderService;
    }
    [HttpGet(ApiEndPointConstant.Order.OrderEndpoint)]
    [ProducesResponseType(typeof(IPaginate<OrderResponse>), statusCode: StatusCodes.Status200OK)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff, RoleEnum.Member)]
    public async Task<IActionResult> GetOrderList([FromQuery] int page = 1, [FromQuery] int size = 30, 
        [FromQuery] OrderFilter? filter = null, [FromQuery] string? sortBy = null, [FromQuery] bool isAsc = true)
    {
        var result = await _orderService.GetOrderList(page, size, filter, sortBy, isAsc);
        return Ok(result);
    }
    [HttpGet(ApiEndPointConstant.Order.OrderItems)]
    [ProducesResponseType(typeof(ICollection<OrderItemResponse>), statusCode: StatusCodes.Status200OK)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff, RoleEnum.Member)]
    public async Task<IActionResult> GetOrderItemsByOrderId(Guid id)
    {
        var result = await _orderService.GetOrderItemsByOrderId(id);
        return Ok(result);
    }
}