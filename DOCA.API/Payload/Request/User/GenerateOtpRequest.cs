using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.User;

public class GenerateOtpRequest
{
    [Required]
    public string PhoneNumber { get; set; }
}