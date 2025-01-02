using DOCA.API.Payload.Request.Cart;
using DOCA.API.Payload.Response.Cart;

namespace DOCA.API.Services.Interface;

public interface ICartService
{
    Task<ICollection<CartModelResponse>> AddToCartAsync(CartModel model);
    // Task<ICollection<CartModelResponse>> GetCartAsync();
    
    // Task<ICollection<CartModelResponse>> RemoveFromCartAsync(Guid productId);
    
    // Task<ICollection<CartModelResponse>> UpdateQuantityAsync(CartModel request);
    //
    // Task<ICollection<CartModelResponse>> ClearCartAsync();
}