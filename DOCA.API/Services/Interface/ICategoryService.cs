using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface ICategoryService
{
    Task<IPaginate<CategoryResponse>> GetCategoriesPagingAsync(int page, int size, CategoryFilter? filter);
    
    Task<CategoryResponse> GetCategoryByIdAsync(Guid id);
    
    Task<CategoryResponse> UpdateProductCategoryByCategoryIdAsync(Guid categoryId, UpdateProductCategoryRequest request);
    
    Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request);
    
    Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request);
}