using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Category;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class CategoryService : BaseService<CategoryService>, ICategoryService
{
    private IConfiguration _configuration;
    public CategoryService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<CategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<CategoryResponse>> GetCategoriesPagingAsync(int page, int size, CategoryFilter? filter)
    {
        var categories = await _unitOfWork.GetRepository<Category>().GetPagingListAsync(
            page: page,
            size: size,
            selector: c => new Category()
            {
                Id = c.Id,
                Name = c.Name,
                Description = c.Description,
                CreatedAt = c.CreatedAt,
                ModifiedAt = c.ModifiedAt
            },
            filter: filter,
            orderBy: c => c.OrderByDescending(c => c.CreatedAt)
        );
        var response = _mapper.Map<IPaginate<CategoryResponse>>(categories);
        return response;
    }

    public async Task<CategoryResponse> GetCategoryByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryIdNotNull);

        var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
            predicate: c => c.Id == id,
            include: c => c.Include(c => c.ProductCategories)
                .ThenInclude(c => c.Product)
        );
        var categoryResponse = _mapper.Map<CategoryResponse>(category);
        return categoryResponse;
    }

    public async Task<CategoryResponse> UpdateProductCategoryByCategoryIdAsync(Guid categoryId, UpdateProductCategoryRequest request)
    {
        if(categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.ProductCategories)
                .ThenInclude(c => c.Product)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.Category.CategoryNotFound);

        var productCategories = await _unitOfWork.GetRepository<ProductCategory>().GetListAsync(
            predicate: pc => pc.CategoryId == categoryId
        );
        var productIds = productCategories.Select(pc => pc.ProductId).ToList();
        var newProductIds = request.ProductIds.Except(productIds).ToList();
        var removeProductIds = productIds.Except(request.ProductIds).ToList();
        foreach (var newProductId in newProductIds)
        {
            var newProduct = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
                predicate:x => x.Id == newProductId,
                include: p => p.Include(p => p.ProductImages)
                    .Include(p => p.ProductCategories)
                    .ThenInclude(pc => pc.Category)
            );
            if (newProduct == null)
            {
                throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
            }
        }
        if(!removeProductIds.Any() && !newProductIds.Any()) return _mapper.Map<CategoryResponse>(category);
        using (var transaction  = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (removeProductIds.Any())
                {
                    var removeProductCategories = await _unitOfWork.GetRepository<ProductCategory>().GetListAsync(
                        predicate: pc => productIds.Contains(pc.ProductId)
                    );
                    foreach (var removeProductCategory in removeProductCategories)
                    {
                        _unitOfWork.GetRepository<ProductCategory>().DeleteAsync(removeProductCategory);
                    }
                }
                
                if (newProductIds.Any())
                {
                    foreach (var newProductId in newProductIds)
                    {
                        await _unitOfWork.GetRepository<ProductCategory>().InsertAsync(
                            new ProductCategory()
                            {
                                ProductId = newProductId,
                                CategoryId = categoryId
                            });
                    }
                }
                
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                CategoryResponse response = null;
                transaction.Complete();
                if(isSuccess) response = _mapper.Map<CategoryResponse>(category);
                return response;
            }
            catch (TransactionException ex)
            {
                _logger.LogError(ex.Message);
                return null;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }

    public async Task<CategoryResponse> UpdateCategoryAsync(Guid categoryId, UpdateCategoryRequest request)
    {
        if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<Category>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.ProductCategories)
                .ThenInclude(c => c.Product)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.Category.CategoryNotFound);
        category.Name = request.Name;
        category.Description = request.Description;
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();
        
        _unitOfWork.GetRepository<Category>().UpdateAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        CategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<CategoryResponse>(category);
        return response;
    }

    public async Task<CategoryResponse> CreateCategoryAsync(CreateCategoryRequest request)
    {
        var category = _mapper.Map<Category>(request);
        category.Id = Guid.NewGuid();
        category.CreatedAt = TimeUtil.GetCurrentSEATime();
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();

        await _unitOfWork.GetRepository<Category>().InsertAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        CategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<CategoryResponse>(category);
        return response;
    }
}