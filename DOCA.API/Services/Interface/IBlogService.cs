using DOCA.API.Payload.Request.Blog;
using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IBlogService
{
    Task<IPaginate<GetBlogDetailResponse>> GetAllBlogPagingAsync(int page, int size, BlogFilter? filter,
        string? sortBy, bool isAsc);

    Task<GetBlogDetailResponse> GetBlogByIdAsync(Guid id);

    Task<GetBlogResponse> CreateBlogAsync(CreateBlogRequest request);

    Task<GetBlogResponse> UpdateBlogByIdAsync(Guid id, UpdateBlogRequest request);

    Task<IPaginate<GetBlogResponse>> GetBlogByBlogCategoryIdAsync(Guid categoryId, int page, int size);
}