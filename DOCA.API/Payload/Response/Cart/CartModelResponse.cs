namespace DOCA.API.Payload.Response.Cart;

public class CartModelResponse
{
    public Guid ProductId { get; set; }
    public Guid BlogId { get; set; }
    public string ProductName { get; set; }
    public string BlogName { get; set; }
    public string ProductDescription { get; set; }
    public string BlogDescription { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }
    public decimal Volume { get; set; }

    public string? MainImage { get; set; }
    
    public int ProductQuantity { get; set; }
}