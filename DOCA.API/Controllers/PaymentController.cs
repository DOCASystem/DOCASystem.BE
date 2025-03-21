using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.CheckOut;
using DOCA.API.Payload.Response.CheckOut;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Payment.PaymentEndPoint)]
public class PaymentController : BaseController<PaymentController>
{
    private readonly IPaymentService _paymentService;
    private readonly ICartService _cartService;
    public PaymentController(ILogger<PaymentController> logger, IPaymentService paymentService, ICartService cartService) : base(logger)
    {
        _paymentService = paymentService;
        _cartService = cartService;
    }
    [HttpPost(ApiEndPointConstant.Payment.PaymentCheckOut)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff, RoleEnum.Member)]
    public async Task<IActionResult> CheckOut([FromBody] CheckOutRequest request)
    {
        var result = await _paymentService.CheckOut(request);
        if (result == null)
        {
            _logger.LogError($"Check out failed");
            return Problem(MessageConstant.Payment.CheckOutFail);
        }
        _logger.LogInformation("Check out successful");
        return CreatedAtAction(nameof(CheckOut), result);
    }
    [HttpPatch(ApiEndPointConstant.Payment.PaymentEndPoint)]
    [ProducesResponseType(typeof(PaymentWithOrderResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff, RoleEnum.Member)]
    public async Task<IActionResult> UpdatePaymentStatus([FromBody] UpdatePaymentOrderStatusRequest request)
    {
        var response = await _paymentService.HandlePayment(request);
        if (response == null)
        {
            _logger.LogError($"Update payment status failed with {request.OrderCode}");
            return Problem($"{MessageConstant.Payment.UpdateStatusPaymentAndOrderFail}: {request.OrderCode}");
        }
        _logger.LogInformation($"Update payment status successful with {request.OrderCode}");
        await _cartService.ClearCartAsync();
        _logger.LogInformation($"Clear cart after order successful");
        return Ok(response);
    }
}