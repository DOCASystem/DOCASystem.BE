using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Product;

public class AddImageProductRequest
{
    [Required]
    public IFormFile ImageUrl { get; set; }
    [Required]
    public bool IsMain { get; set; }
}