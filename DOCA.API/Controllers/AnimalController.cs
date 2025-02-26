using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Validators;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;
using Microsoft.AspNetCore.Mvc;

namespace DOCA.API.Controllers;

[ApiController]
[Route(ApiEndPointConstant.Animal.AnimalEndpoint)]
public class AnimalController : BaseController<AnimalController>
{
    private readonly IAnimalService _animalService;
    public AnimalController(ILogger<AnimalController> logger, IAnimalService animalService) : base(logger)
    {
        _animalService = animalService;
    }
    
    [HttpGet(ApiEndPointConstant.Animal.AnimalEndpoint)]
    [ProducesResponseType(typeof(IPaginate<GetAnimalDetailResponse>), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAllAnimal(int page = 1, int size = 30,[FromQuery] AnimalFilter? filter = null, string? sortBy = null, bool isAsc = true)
    {
        var response = await _animalService.GetAllAnimalPagingAsync(page, size, filter, sortBy, isAsc);
        return Ok(response);
    }
    
    [HttpGet(ApiEndPointConstant.Animal.AnimalById)]
    [ProducesResponseType(typeof(GetProductDetailResponse), statusCode: StatusCodes.Status200OK)]
    public async Task<IActionResult> GetAnimalById(Guid id)
    {
        var response = await _animalService.GetAnimalByIdAsync(id);
        return Ok(response);
    }
    
    [HttpPost(ApiEndPointConstant.Animal.AnimalEndpoint)]
    [ProducesResponseType(typeof(GetAnimalResponse), statusCode: StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> CreateAnimal([FromForm] CreateAnimalRequest request)
    {
        var response = await _animalService.CreateAnimalAsync(request);
        if (response == null)
        {
            _logger.LogError($"Create new animal failed with {request.Name}");
            return Problem($"{MessageConstant.Product.CreateProductFail}: {request.Name}");
        }
        _logger.LogInformation($"Create new animal successful with {request.Name}");
        return CreatedAtAction(nameof(CreateAnimal), response);
    }
    
    [HttpPatch(ApiEndPointConstant.Animal.AnimalById)]
    [ProducesResponseType(typeof(GetAnimalResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateAnimalById(Guid id, [FromBody] UpdateAnimalRequest request)
    {
        var response = await _animalService.UpdateAnimalByIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update animal failed with {id}");
            return Problem($"{MessageConstant.Product.UpdateProductFail}: {id}");
        }
        _logger.LogInformation($"Update animal successful with {id}");
        return Ok(response);
    }
    
    [HttpPatch(ApiEndPointConstant.Animal.AnimalImage)]
    [ProducesResponseType(typeof(GetAnimalResponse), statusCode: StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), statusCode: StatusCodes.Status500InternalServerError)]
    [CustomAuthorize(RoleEnum.Manager, RoleEnum.Staff)]
    public async Task<IActionResult> UpdateAnimalImage(Guid id, [FromBody] ICollection<ImageAnimalRequest> request)
    {
        var response = await _animalService.UpdateAnimalImageByAnimalIdAsync(id, request);
        if (response == null)
        {
            _logger.LogError($"Update animal image failed with {id}");
            return Problem($"{MessageConstant.ProductImage.AddProductImageFail}: {id}");
        }
        _logger.LogInformation($"Update animal image successful with {id}");
        return Ok(response);
    }
    
}