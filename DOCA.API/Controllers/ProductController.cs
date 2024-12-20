using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Product.ProductEndpoint)]
public class ProductController : BaseController<ProductController>
{
    private readonly IProductService _productService;
    public ProductController(ILogger<ProductController> logger, IProductService productService) : base(logger)
    {
        _productService = productService;
    }
    
    [HttpGet(ApiEndPointConstant.Product.ProductEndpoint)]
    [ProducesResponseType(typeof(IPaginate<GetProductDetailResponse>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllProduct(int page = 1, int size = 30,[FromQuery] ProductFilter? filter = null, string? sortBy = null, bool isAsc = true)
    {
        var response = await _productService.GetAllProductPagingAsync(page, size, filter, sortBy, isAsc);
        return Ok(response);
    }
    
    [HttpGet(ApiEndPointConstant.Product.ProductById)]
    [ProducesResponseType(typeof(GetProductDetailResponse), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetProductById(Guid id)
    {
        var response = await _productService.GetProductByIdAsync(id);
        return Ok(response);
    }
    
    [HttpPost(ApiEndPointConstant.Product.ProductEndpoint)]
    [ProducesResponseType(typeof(GetProductResponse), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> CreateProduct([FromBody] CreateProductRequest request)
    {
        var response = await _productService.CreateProductAsync(request);
        if (response == null)
        {
            _logger.LogError($"Create new product failed with {request.Name}");
            return Problem($"{MessageConstant.Product.CreateProductFail}: {request.Name}");
        }
        _logger.LogInformation($"Create new product successful with {request.Name}");
        return CreatedAtAction(nameof(CreateProduct), response);
    }
    
    [HttpPatch(ApiEndPointConstant.Product.ProductById)]
    [ProducesResponseType(typeof(GetProductResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateProductById(Guid id, [FromBody] UpdateProductRequest request)
    {
        var response = await _productService.UpdateProductByIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update product failed with {id}");
            return Problem($"{MessageConstant.Product.UpdateProductFail}: {id}");
        }
        _logger.LogInformation($"Update product successful with {id}");
        return Ok(response);
    }
    
    [HttpPatch(ApiEndPointConstant.Product.ProductImage)]
    [ProducesResponseType(typeof(GetProductResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateProductImage(Guid id, [FromBody] ICollection<ImageProductRequest> request)
    {
        var response = await _productService.UpdateProductImageByProductIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update product image failed with {id}");
            return Problem($"{MessageConstant.ProductImage.AddProductImageFail}: {id}");
        }
        _logger.LogInformation($"Update product image successful with {id}");
        return Ok(response);
    }
}