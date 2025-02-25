using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Product;

public class CreateProductRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public decimal Price { get; set; }
    [Required]
    public int Quantity { get; set; }
    [Required]
    public decimal Volume { get; set; }
    [Required]
    public bool IsHidden { get; set; }
    
    public List<Guid>? CategoryIds { get; set; }
    [Required]
    public string MainImage { get; set; }
    public List<string>? SecondaryImages { get; set; }
}