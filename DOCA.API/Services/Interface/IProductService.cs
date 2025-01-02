using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Product;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IProductService
{
    Task<IPaginate<GetProductDetailResponse>> GetAllProductPagingAsync(int page, int size, ProductFilter? filter,
        string? sortBy, bool isAsc);

    Task<GetProductDetailResponse> GetProductByIdAsync(Guid id);

    Task<GetProductResponse> CreateProductAsync(CreateProductRequest request);

    Task<GetProductResponse> UpdateProductByIdAsync(Guid id, UpdateProductRequest request);

    Task<IPaginate<GetProductResponse>> GetProductByCategoryIdAsync(Guid categoryId, int page, int size);

    Task<GetProductResponse> DeleteProductImageById(Guid id);

    Task<GetProductResponse> UpdateProductImageByProductIdAsync(Guid productId, ICollection<ImageProductRequest> request);
}