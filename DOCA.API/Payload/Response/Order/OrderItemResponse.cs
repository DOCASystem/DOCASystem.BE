using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.Product;

namespace DOCA.API.Payload.Response.Order;

public class OrderItemResponse
{
    public Guid Id { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public GetProductResponse Product { get; set; }
    public GetBlogResponse Blog { get; set; }
}