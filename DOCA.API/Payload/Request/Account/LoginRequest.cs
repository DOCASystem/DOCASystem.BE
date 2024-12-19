using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Account;

public class LoginRequest
{
    [Required]
    public string UsernameOrPhoneNumber { get; set; }
    [Required]
    public string Password { get; set; }
}