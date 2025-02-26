using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Product;

public class ImageProductRequest
{
    public Guid? Id { get; set; }
    [Required]
    public IFormFile ImageUrl { get; set; }
    [Required]
    public bool IsMain { get; set; }
}