using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.User;

public class ForgetPasswordRequest
{
    [Required]
    public string Otp { get; set; }
    [Required]
    public string PhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
}