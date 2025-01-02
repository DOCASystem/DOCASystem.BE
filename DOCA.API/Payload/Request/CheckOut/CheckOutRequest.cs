using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.CheckOut;

public class CheckOutRequest
{
    [Required]
    public string Address { get; set; }
}