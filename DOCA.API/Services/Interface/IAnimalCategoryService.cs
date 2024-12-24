using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IAnimalCategoryService
{
    Task<IPaginate<AnimalCategoryResponse>> GetAnimalCategoriesPagingAsync(int page, int size, AnimalCategoryFilter? filter);
    
    Task<AnimalCategoryResponse> GetAnimalCategoryByIdAsync(Guid id);
    
    Task<AnimalCategoryResponse> UpdateAnimalCategoryByCategoryIdAsync(Guid categoryId, UpdateAnimalCategoryRelationshipRequest request);
    
    Task<AnimalCategoryResponse> UpdateAnimalCategoryAsync(Guid categoryId, UpdateAnimalCategoryRequest request);
    
    Task<AnimalCategoryResponse> CreateAnimalCategoryAsync(CreateAnimalCategoryRequest request);
}