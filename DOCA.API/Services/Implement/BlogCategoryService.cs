using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Payload.BlogCategory;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class BlogCategoryService : BaseService<BlogCategoryService>, IBlogCategoryService
{
    private IConfiguration _configuration;
    public BlogCategoryService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<BlogCategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<BlogCategoryResponse>> GetBlogCategoriesPagingAsync(int page, int size, BlogCategoryFilter? filter)
    {
        var categories = await _unitOfWork.GetRepository<BlogCategory>().GetPagingListAsync(
            page: page,
            size: size,
            selector: c => new BlogCategory()
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
        var response = _mapper.Map<IPaginate<BlogCategoryResponse>>(categories);
        return response;
    }

    public async Task<BlogCategoryResponse> GetBlogCategoryByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryIdNotNull);

        var category = await _unitOfWork.GetRepository<BlogCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == id,
            include: c => c.Include(c => c.BlogCategoryRelationship)
                .ThenInclude(c => c.BLog)
        );
        var categoryResponse = _mapper.Map<BlogCategoryResponse>(category);
        return categoryResponse;
    }

    public async Task<BlogCategoryResponse> UpdateBlogCategoryRelationshipByBlogCategoryIdAsync(Guid categoryId,
        UpdateBlogCategoryRelationshipRequest request)
    {
         if(categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<BlogCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.BlogCategoryRelationship)
                .ThenInclude(c => c.BLog)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryNotFound);

        var blogCategories = await _unitOfWork.GetRepository<BlogCategoryRelationship>().GetListAsync(
            predicate: pc => pc.BlogCategoryId == categoryId
        );
        var blogIds = blogCategories.Select(pc => pc.BlogId).ToList();
        var newBloglIds = request.BlogIds.Except(blogIds).ToList();
        var removeAnimalIds = blogIds.Except(request.BlogIds).ToList();
        foreach (var newBloglId in newBloglIds)
        {
            var newBlog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
                predicate:x => x.Id == newBloglId,
                include: p => p.Include(p => p.BlogCategoryRelationship)
                    .ThenInclude(pc => pc.BlogCategory)
            );
            if (newBlog == null)
            {
                throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryNotFound);
            }
        }
        if(!removeAnimalIds.Any() && !newBloglIds.Any()) return _mapper.Map<BlogCategoryResponse>(category);
        using (var transaction  = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (removeAnimalIds.Any())
                {
                    var removeBlogCategories = await _unitOfWork.GetRepository<BlogCategoryRelationship>().GetListAsync(
                        predicate: pc => newBloglIds.Contains(pc.BlogId)
                    );
                    foreach (var removeBlogCategory in removeBlogCategories)
                    {
                        _unitOfWork.GetRepository<BlogCategoryRelationship>().DeleteAsync(removeBlogCategory);
                    }
                }
                
                if (newBloglIds.Any())
                {
                    foreach (var newBloglId in newBloglIds)
                    {
                        await _unitOfWork.GetRepository<BlogCategoryRelationship>().InsertAsync(
                            new BlogCategoryRelationship()
                            {
                                BlogId = newBloglId,
                                BlogCategoryId = categoryId
                            });
                    }
                }
                
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                BlogCategoryResponse response = null;
                transaction.Complete();
                if(isSuccess) response = _mapper.Map<BlogCategoryResponse>(category);
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

    public async Task<BlogCategoryResponse> UpdateBlogCategoryAsync(Guid categoryId, UpdateBlogCategoryRequest request)
    {
        if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<BlogCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.BlogCategoryRelationship)
                .ThenInclude(c => c.BLog)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryNotFound);
        category.Name = request.Name;
        category.Description = request.Description;
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();
        
        _unitOfWork.GetRepository<BlogCategory>().UpdateAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        BlogCategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<BlogCategoryResponse>(category);
        return response;
    }

    public async Task<BlogCategoryResponse> CreateBlogCategoryAsync(CreateBlogCategoryRequest request)
    {
        var category = _mapper.Map<BlogCategory>(request);
        category.Id = Guid.NewGuid();
        category.CreatedAt = TimeUtil.GetCurrentSEATime();
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();

        await _unitOfWork.GetRepository<BlogCategory>().InsertAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        BlogCategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<BlogCategoryResponse>(category);
        return response;
    }
}