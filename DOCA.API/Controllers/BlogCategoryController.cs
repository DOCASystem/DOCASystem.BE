using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.BlogCategory;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.BlogCategory.CategoryEndPoint)]
public class BlogCategoryController : BaseController<BlogCategoryController>
{
    private readonly IBlogCategoryService _blogCategoryService;
    private readonly IBlogService _blogService;
    public BlogCategoryController(ILogger<BlogCategoryController> logger, IBlogCategoryService blogCategoryService, IBlogService blogService) : base(logger)
    {
        _blogCategoryService = blogCategoryService;
        _blogService = blogService;
    }
    [HttpGet(ApiEndPointConstant.BlogCategory.CategoryEndPoint)]
    [ProducesResponseType(typeof(IPaginate<BlogCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlogCategoriesPagingAsync(int page = 1, int size = 30, [FromQuery] BlogCategoryFilter filter = null)
    {
        var categories = await _blogCategoryService.GetBlogCategoriesPagingAsync(page, size, filter);
        return Ok(categories);
    }
    [HttpGet(ApiEndPointConstant.BlogCategory.CategoryById)]
    [ProducesResponseType(typeof(BlogCategoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlogCategoryByIdAsync(Guid id)
    {
        var category = await _blogCategoryService.GetBlogCategoryByIdAsync(id);
        return Ok(category);
    }
    [HttpPatch(ApiEndPointConstant.BlogCategory.UpdateAnimalCategory)]
    [ProducesResponseType(typeof(BlogCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateBlogCategoryByCategoryIdAsync(Guid id, [FromBody] UpdateBlogCategoryRelationshipRequest request)
    {
        var response = await _blogCategoryService.UpdateBlogCategoryRelationshipByBlogCategoryIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update blog category failed with {id}");
            return Problem($"{MessageConstant.Category.UpdateProductCategoryFail}: {id}");
        }
        _logger.LogInformation($"Update blog category successful with {id}");
        return Ok(response);
    }

    [HttpPatch(ApiEndPointConstant.BlogCategory.CategoryById)]
    [ProducesResponseType(typeof(BlogCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateBlogCategoryAsync(Guid id, [FromBody] UpdateBlogCategoryRequest request)
    {
        var response = await _blogCategoryService.UpdateBlogCategoryAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update blog category failed with {id}");
            return Problem($"{MessageConstant.Category.UpdateCategoryFail}: {id}");
        }

        _logger.LogInformation($"Update blog category successful with {id}");
        return Ok(response);
    }
    [HttpPost(ApiEndPointConstant.BlogCategory.CategoryEndPoint)]
    [ProducesResponseType(typeof(BlogCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> CreateBlogCategory([FromBody] CreateBlogCategoryRequest request)
    {
        var response = await _blogCategoryService.CreateBlogCategoryAsync(request);
        if (response == null)
        {
            _logger.LogError($"Create new blog category failed with {request.Name}");
            return Problem($"{MessageConstant.Category.CreateCategoryFail}: {request.Name}");
        }
        _logger.LogInformation($"Create new blog category successful with {request.Name}");
        return CreatedAtAction(nameof(CreateBlogCategory), response);
    }
    [HttpGet(ApiEndPointConstant.BlogCategory.BlogByBlogCategoryId)]
    [ProducesResponseType(typeof(IPaginate<GetBlogResponse>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetBlogsByBlogCategoryId(Guid id = new Guid(), int page = 1, int size = 30)
    {
        var response = await _blogService.GetBlogByBlogCategoryIdAsync(id, page, size);
        return Ok(response);
    }
}