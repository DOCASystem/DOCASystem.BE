using DOCA.API.Payload.Request.CheckOut;
using DOCA.API.Payload.Response.CheckOut;

namespace DOCA.API.Services.Interface;

public interface IPaymentService
{
    Task<string> CheckOut(CheckOutRequest request);
    Task<PaymentWithOrderResponse> HandlePayment(UpdatePaymentOrderStatusRequest request);

    Task<bool> UpdateExpiredPayment();
}