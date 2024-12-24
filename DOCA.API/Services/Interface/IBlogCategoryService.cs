using DOCA.API.Payload.BlogCategory;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.API.Payload.Response.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IBlogCategoryService
{
    Task<IPaginate<BlogCategoryResponse>> GetBlogCategoriesPagingAsync(int page, int size, BlogCategoryFilter? filter);
    
    Task<BlogCategoryResponse> GetBlogCategoryByIdAsync(Guid id);
    
    Task<BlogCategoryResponse> UpdateBlogCategoryRelationshipByBlogCategoryIdAsync(Guid categoryId, UpdateBlogCategoryRelationshipRequest request);
    
    Task<BlogCategoryResponse> UpdateBlogCategoryAsync(Guid categoryId, UpdateBlogCategoryRequest request);
    
    Task<BlogCategoryResponse> CreateBlogCategoryAsync(CreateBlogCategoryRequest request);
}