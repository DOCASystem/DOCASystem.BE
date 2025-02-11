using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.User;

public class GenerateEmailOtpRequest
{
    [Required]
    public string Email { get; set; }
}