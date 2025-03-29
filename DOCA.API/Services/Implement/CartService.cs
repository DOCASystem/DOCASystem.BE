using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Payload.Request.Cart;
using DOCA.API.Payload.Response.Cart;
using DOCA.API.Services.Interface;
using DOCA.Domain.Models;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace DOCA.API.Services.Implement;

public class CartService : BaseService<CartService>, ICartService
{
    private IConfiguration _configuration;
    private readonly IRedisService _redisService;
    public CartService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<CartService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IRedisService redisService) : base(unitOfWork, logger, mapper, httpContextAccessor,configuration)
    {
        _configuration = configuration;
        _redisService = redisService;
    }

    public async Task<ICollection<CartModelResponse>> AddToCartAsync(CartModel request)
{
    var userId = GetUserIdFromJwt();
    if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
    
    var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
        predicate: x => x.Id == userId
    );
    if (user == null) throw new BadHttpRequestException(MessageConstant.User.UserNotFound);

    var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
        predicate: x => x.Id == request.ProductId,
        include: p => p
            .Include(p => p.ProductImages)
            .Include(p => p.ProductCategories)
            .ThenInclude(pc => pc.Category)
    );
    if (product == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);

    var blog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
        predicate: x => x.Id == request.BlogId,
        include: b => b
            .Include(b => b.BlogCategoryRelationship)
            .ThenInclude(b => b.BlogCategory)
            .Include(b => b.BlogAnimal)
            .ThenInclude(b => b.Animal)
    );
    if (blog == null) throw new BadHttpRequestException(MessageConstant.Blog.BlogNotFound);
    
    var response = new CartModelResponse
    {
        ProductId = product.Id,
        ProductName = product.Name,
        ProductDescription = product.Description,
        Price = product.Price,
        Volume = product.Volume,

        BlogId = blog.Id,
        BlogName = blog.Name,
        BlogDescription = blog.Description,

        Quantity = request.Quantity,
        MainImage = product.ProductImages?.FirstOrDefault(pi => pi.IsMain)?.ImageUrl,
        ProductQuantity = product.Quantity
    };

    var key = "Cart:" + userId;
    var cartData = await _redisService.GetStringAsync(key);
    List<CartModelResponse> cart = new();

    if (!string.IsNullOrEmpty(cartData))
    {
        cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData, new JsonSerializerSettings
        {
            FloatParseHandling = FloatParseHandling.Decimal
        });
    }
    else
    {
        await _redisService.PushToListAsync("AllCartKeys", key);
    }

    var existedProduct = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
    if (existedProduct != null)
    {
        existedProduct.Quantity += request.Quantity;
        if (existedProduct.Quantity > product.Quantity)
            throw new BadHttpRequestException(MessageConstant.Product.ProductOutOfStock);
    }
    else
    {
        if (request.Quantity > product.Quantity)
            throw new BadHttpRequestException(MessageConstant.Product.ProductOutOfStock);
        cart.Add(response);
    }

    var updatedCart = JsonConvert.SerializeObject(cart, new JsonSerializerSettings
    {
        FloatFormatHandling = FloatFormatHandling.String, 
        Formatting = Formatting.None
    });

    var isSuccess = await _redisService.SetStringAsync(key, updatedCart);
    return isSuccess ? cart : null;
}


    public async Task<ICollection<CartModelResponse>> GetCartAsync()
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        
        var key = "Cart:" + userId;
        
        var cartData = await _redisService.GetStringAsync(key);
    
        if (string.IsNullOrEmpty(cartData))
        {
            return new List<CartModelResponse>();
        }
        var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData);
        return cart;
    }

    public async Task<ICollection<CartModelResponse>> RemoveFromCartAsync(Guid productId)
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
    
        if (productId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.ProductIdNotNull);
        // var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
        // var db = redis.GetDatabase();
        var key = "Cart:" + userId;
        var cartData = await _redisService.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cartData))
        {
            return new List<CartModelResponse>();
        }
        var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData);
        var product = cart.FirstOrDefault(x => x.ProductId == productId);
    
        if (product == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
        cart.Remove(product);
        if (!cart.Any())
        {
            await _redisService.RemoveKeyAsync(key);
            await _redisService.RemoveFromListAsync("AllCartKeys", key);
        }
        else
        {
            var updatedCart = JsonConvert.SerializeObject(cart);
            await _redisService.SetStringAsync(key, updatedCart);
        }
    
        return cart;
    }

    public async Task<ICollection<CartModelResponse>> UpdateQuantityAsync(CartModel request)
    {
        if (request.ProductId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.ProductIdNotNull);
        if (request.Quantity <= 0) throw new BadHttpRequestException(MessageConstant.Cart.QuantityMustBeGreaterThanZero);
        
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
    
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: x => x.Id == request.ProductId,
            include: p => p
                .Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
        );
        if(product == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
        if (request.Quantity > product.Quantity)
            throw new BadHttpRequestException(MessageConstant.Product.ProductOutOfStock);
        var key = "Cart:" + userId;
        var cartData = await _redisService.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cartData))
        {
            return new List<CartModelResponse>();
        }
        var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData);
        var existedProduct = cart.FirstOrDefault(x => x.ProductId == request.ProductId);
        if (existedProduct == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
        existedProduct.Quantity = request.Quantity;
        var updatedCart = JsonConvert.SerializeObject(cart);
        await _redisService.SetStringAsync(key, updatedCart);
        return cart;
    }

    public async Task<ICollection<CartModelResponse>> ClearCartAsync()
    {
        var userId = GetUserIdFromJwt();
        if (userId == Guid.Empty) throw new UnauthorizedAccessException(MessageConstant.User.UserNotFound);
        var user = await _unitOfWork.GetRepository<User>().SingleOrDefaultAsync(
            predicate: x => x.Id == userId
        );
        if (user == null) throw new BadHttpRequestException(MessageConstant.User.UserNotFound);
        // var redis = ConnectionMultiplexer.Connect(_configuration.GetConnectionString("Redis"));
        // var db = redis.GetDatabase();
        var key = "Cart:" + userId;
        var cartData = await _redisService.GetStringAsync(key);
        
        if (string.IsNullOrEmpty(cartData))
        {
            return null;
        }
        var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartData);
        cart.Clear();
        if (!cart.Any())
        {
            await _redisService.RemoveKeyAsync(key);
            await _redisService.RemoveFromListAsync("AllCartKeys", key);
        }
        return cart;
    }
}