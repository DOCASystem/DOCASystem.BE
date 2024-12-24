using System.Collections;
using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Blog;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Blog;
using DOCA.API.Payload.Response.BlogCategory;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class BlogService : BaseService<BlogService>, IBlogService
{
    private IConfiguration _configuration;
    public BlogService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<BlogService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<GetBlogDetailResponse>> GetAllBlogPagingAsync(int page, int size, BlogFilter? filter, string? sortBy, bool isAsc)
    {
        var blogs = await _unitOfWork.GetRepository<Blog>()
            .GetPagingListAsync(
                selector: a => new Blog()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    CreatedAt = a.CreatedAt,
                    ModifiedAt = a.ModifiedAt,
                    IsHindden = a.IsHindden,
                    BlogCategoryRelationship = 
                        a.BlogCategoryRelationship.Any(bcr => bcr.BlogId == a.Id) ? a.BlogCategoryRelationship : null,
                }, page: page, size: size,
                include: a => a.Include(a => a.BlogCategoryRelationship).ThenInclude(arc => arc.BlogCategory), 
                filter: filter,
                sortBy: sortBy, isAsc: isAsc);
        var response = _mapper.Map<IPaginate<GetBlogDetailResponse>>(blogs);
        return response;
    }

    public async Task<GetBlogDetailResponse> GetBlogByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Blog.BlogIdNotNull);
        var role = GetRoleFromJwt();
        var b = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(predicate: b => b.Id.Equals(id));
        if ( role != RoleEnum.Manager && role != RoleEnum.Staff)
            throw new BadHttpRequestException(MessageConstant.Blog.BlogNotFound);
        var response = new GetBlogDetailResponse()
        {
            Id = b.Id,
            Name = b.Name,
            Description = b.Description,
            CreatedAt = b.CreatedAt,
            ModifiedAt = b.ModifiedAt,
            IsHidden = b.IsHindden,
            BlogCategories = b.BlogCategoryRelationship.Select(ac => ac.BlogCategory)
                .Select(c => new BlogCategoryResponse()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.ModifiedAt,
                })
                .ToList()
        };
        return response;
    }

    public async Task<GetBlogResponse> CreateBlogAsync(CreateBlogRequest request)
    {
        var blog = _mapper.Map<Blog>(request);
        blog.Id = Guid.NewGuid();
        blog.CreatedAt = TimeUtil.GetCurrentSEATime();
        blog.ModifiedAt = TimeUtil.GetCurrentSEATime();
        decimal expectedPrice = 0;
        if (request.BlogCategoryIds != null)
        {
            foreach (var blogCategoryId in request.BlogCategoryIds)
            {
                var category = await _unitOfWork.GetRepository<BlogCategory>()
                    .SingleOrDefaultAsync(predicate: c => c.Id.Equals(blogCategoryId));
                if (category == null) throw new BadHttpRequestException(MessageConstant.BlogCategory.BlogCategoryNotFound);
            }
        }

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (request.BlogCategoryIds != null)
                {
                    foreach (var blogCategoryId in request.BlogCategoryIds)
                    {
                        await _unitOfWork.GetRepository<BlogCategoryRelationship>()
                            .InsertAsync(new BlogCategoryRelationship() { BlogId = blog.Id, BlogCategoryId = blogCategoryId });
                    }
                }
                await _unitOfWork.GetRepository<Blog>().InsertAsync(blog);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess) return null;
                transaction.Complete();
                return _mapper.Map<GetBlogResponse>(blog);
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

    public async Task<GetBlogResponse> UpdateBlogByIdAsync(Guid id, UpdateBlogRequest request)
    {
        if(id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Blog.BlogIdNotNull);

        var blog = await _unitOfWork.GetRepository<Blog>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(id)
        );
        if(blog == null) throw new BadHttpRequestException(MessageConstant.Blog.BlogNotFound);
        blog.Name = string.IsNullOrEmpty(request.Name) ? blog.Name : request.Name;
        blog.Description = string.IsNullOrEmpty(request.Description) ? blog.Description : request.Description;
        blog.IsHindden = (bool) (request.IsHidden == null ? blog.IsHindden : request.IsHidden);
        blog.ModifiedAt = TimeUtil.GetCurrentSEATime();
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                _unitOfWork.GetRepository<Blog>().UpdateAsync(blog);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                transactionScope.Complete();
                GetBlogResponse response = null;
                if (isSuccess)
                {
                    response = _mapper.Map<GetBlogResponse>(blog);
                    // var cartKeys = await _redisService.GetListAsync("AllCartKeys");
                    // if (cartKeys.Any())
                    // {
                    //     foreach (var cartKey in cartKeys)
                    //     {
                    //         var cartJson = await _redisService.GetStringAsync(cartKey);
                    //         var cart = JsonConvert.DeserializeObject<List<CartModelResponse>>(cartJson);
                    //         foreach (var cartItem in cart)
                    //         {
                    //             if (cartItem.ProductId == product.Id)
                    //             {
                    //                 
                    //                 if (cartItem.Quantity > product.Quantity || product.IsHidden)
                    //                 {
                    //                     cart.Remove(cartItem);
                    //                     break;
                    //                 }
                    //                 cartItem.Name = product.Name;
                    //                 cartItem.Description = product.Description;
                    //                 cartItem.Price = product.Price;
                    //                 cartItem.MainImage = product.ProductImages?.Where(pi => pi.IsMain == true).FirstOrDefault()?.ImageUrl;
                    //                 cartItem.ProductQuantity = product.Quantity;
                    //             }
                    //         }
                    //         if (!cart.Any())
                    //         {
                    //             await _redisService.RemoveKeyAsync(cartKey);
                    //             await _redisService.RemoveFromListAsync("AllCartKeys", cartKey);
                    //         }
                    //         else
                    //         {
                    //             await _redisService.SetStringAsync(cartKey, JsonConvert.SerializeObject(cart));
                    //         }
                    //     }
                    // }
                }
                return response;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }

    public async Task<IPaginate<GetBlogResponse>> GetBlogByBlogCategoryIdAsync(Guid categoryId, int page, int size)
    {
        if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Blog.BlogIdNotNull);
    
        var blogs = await _unitOfWork.GetRepository<Blog>().GetPagingListAsync(
            selector: a => new Blog()
            {
                Id = a.Id,
                Description = a.Description,
                Name = a.Name,
                CreatedAt = a.CreatedAt,
                ModifiedAt = a.ModifiedAt,
                IsHindden = a.IsHindden,
                BlogCategoryRelationship = a.BlogCategoryRelationship.Any(bcr => bcr.BlogId == a.Id) ? a.BlogCategoryRelationship : null,
            },
            predicate: a => a.BlogCategoryRelationship.Any(pc => pc.BlogCategoryId == categoryId),
            page: page,
            size: size,
            filter: null
        );
        var responses = _mapper.Map<IPaginate<GetBlogResponse>>(blogs);
        return responses;
    }
}