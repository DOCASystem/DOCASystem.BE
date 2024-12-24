using AutoMapper;
using DOCA.API.Payload.BlogCategory;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.API.Services.Interface;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;

namespace DOCA.API.Services.Implement;

public class BlogCategoryService : BaseService<BlogCategoryService>, IBlogCategoryService
{
    private IConfiguration _configuration;
    public BlogCategoryService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<BlogCategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public Task<IPaginate<BlogCategoryResponse>> GetBlogCategoriesPagingAsync(int page, int size, BlogCategoryFilter? filter)
    {
        throw new NotImplementedException();
    }

    public Task<BlogCategoryResponse> GetBlogCategoryByIdAsync(Guid id)
    {
        throw new NotImplementedException();
    }

    public Task<BlogCategoryResponse> UpdateBlogCategoryRelationshipByBlogCategoryIdAsync(Guid categoryId,
        UpdateBlogCategoryRelationshipRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BlogCategoryResponse> UpdateBlogCategoryAsync(Guid categoryId, UpdateBlogCategoryRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<BlogCategoryResponse> CreateBlogCategoryAsync(CreateBlogCategoryRequest request)
    {
        throw new NotImplementedException();
    }
}