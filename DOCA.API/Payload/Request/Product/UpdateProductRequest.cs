namespace DOCA.API.Payload.Request.Product;

public class UpdateProductRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public int? Quantity { get; set; }
    public decimal? Volume { get; set; }
    public decimal? Price { get; set; }
    public bool? IsHidden { get; set; }
}