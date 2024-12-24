using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Blog;
using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Blog.BlogEndpoint)]
public class BlogController : BaseController<BlogController>
{
    private readonly IBlogService _blogService;
    public BlogController(ILogger<BlogController> logger, IBlogService blogService) : base(logger)
    {
        _blogService = blogService;
    }
    
    [HttpGet(ApiEndPointConstant.Blog.BlogEndpoint)]
    [ProducesResponseType(typeof(IPaginate<GetBlogDetailResponse>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllBlog(int page = 1, int size = 30,[FromQuery] BlogFilter? filter = null, string? sortBy = null, bool isAsc = true)
    {
        var response = await _blogService.GetAllBlogPagingAsync(page, size, filter, sortBy, isAsc);
        return Ok(response);
    }
    
    [HttpGet(ApiEndPointConstant.Blog.BlogById)]
    [ProducesResponseType(typeof(GetBlogDetailResponse), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlogById(Guid id)
    {
        var response = await _blogService.GetBlogByIdAsync(id);
        return Ok(response);
    }
    
    [HttpPost(ApiEndPointConstant.Blog.BlogEndpoint)]
    [ProducesResponseType(typeof(GetBlogResponse), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> CreateBlog([FromBody] CreateBlogRequest request)
    {
        var response = await _blogService.CreateBlogAsync(request);
        if (response == null)
        {
            _logger.LogError($"Create new blog failed with {request.Name}");
            return Problem($"{MessageConstant.Product.CreateProductFail}: {request.Name}");
        }
        _logger.LogInformation($"Create new blog successful with {request.Name}");
        return CreatedAtAction(nameof(CreateBlog), response);
    }
    
    [HttpPatch(ApiEndPointConstant.Blog.BlogById)]
    [ProducesResponseType(typeof(GetBlogResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateBlogById(Guid id, [FromBody] UpdateBlogRequest request)
    {
        var response = await _blogService.UpdateBlogByIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update blog failed with {id}");
            return Problem($"{MessageConstant.Product.UpdateProductFail}: {id}");
        }
        _logger.LogInformation($"Update blog successful with {id}");
        return Ok(response);
    }
}