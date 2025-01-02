namespace DOCA.API.Payload.Request.Cart;

public class CartModel
{
    public Guid ProductId { get; set; }
    public Guid BlogId { get; set; }
    public int Quantity { get; set; }
}