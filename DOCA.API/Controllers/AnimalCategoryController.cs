using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.AnimalCategory.CategoryEndPoint)]
public class AnimalCategoryController : BaseController<AnimalCategoryController>
{
    private readonly IAnimalCategoryService _animalCategoryService;
    public AnimalCategoryController(ILogger<AnimalCategoryController> logger, IAnimalCategoryService animalCategoryService) : base(logger)
    {
        _animalCategoryService = animalCategoryService;
    }
    [HttpGet(ApiEndPointConstant.AnimalCategory.CategoryEndPoint)]
    [ProducesResponseType(typeof(IPaginate<AnimalCategoryResponse>), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnimalCategoriesPagingAsync(int page = 1, int size = 30, [FromQuery] AnimalCategoryFilter filter = null)
    {
        var categories = await _animalCategoryService.GetAnimalCategoriesPagingAsync(page, size, filter);
        return Ok(categories);
    }
    [HttpGet(ApiEndPointConstant.AnimalCategory.CategoryById)]
    [ProducesResponseType(typeof(AnimalCategoryResponse), StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnimalCategoryByIdAsync(Guid id)
    {
        var category = await _animalCategoryService.GetAnimalCategoryByIdAsync(id);
        return Ok(category);
    }
    [HttpPatch(ApiEndPointConstant.AnimalCategory.UpdateAnimalCategory)]
    [ProducesResponseType(typeof(AnimalCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateAnimalCategoryByCategoryIdAsync(Guid id, [FromBody] UpdateAnimalCategoryRelationshipRequest request)
    {
        var response = await _animalCategoryService.UpdateAnimalCategoryByCategoryIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update animal category failed with {id}");
            return Problem($"{MessageConstant.Category.UpdateProductCategoryFail}: {id}");
        }
        _logger.LogInformation($"Update animal category successful with {id}");
        return Ok(response);
    }

    [HttpPatch(ApiEndPointConstant.AnimalCategory.CategoryById)]
    [ProducesResponseType(typeof(AnimalCategoryResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateAnimalCategoryAsync(Guid id, [FromBody] UpdateAnimalCategoryRequest request)
    {
        var response = await _animalCategoryService.UpdateAnimalCategoryAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update animal category failed with {id}");
            return Problem($"{MessageConstant.Category.UpdateCategoryFail}: {id}");
        }

        _logger.LogInformation($"Update animal category successful with {id}");
        return Ok(response);
    }
    [HttpPost(ApiEndPointConstant.AnimalCategory.CategoryEndPoint)]
    [ProducesResponseType(typeof(AnimalCategoryResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> CreateAnimalCategory([FromBody] CreateAnimalCategoryRequest request)
    {
        var response = await _animalCategoryService.CreateAnimalCategoryAsync(request);
        if (response == null)
        {
            _logger.LogError($"Create new animal category failed with {request.Name}");
            return Problem($"{MessageConstant.Category.CreateCategoryFail}: {request.Name}");
        }
        _logger.LogInformation($"Create new animal category successful with {request.Name}");
        return CreatedAtAction(nameof(CreateAnimalCategory), response);
    }
}