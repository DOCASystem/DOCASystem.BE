using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.CheckOut;

public class UpdatePaymentOrderStatusRequest
{
    [Required]
    public int OrderCode { get; set; }
}