using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Product;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class ProductService : BaseService<ProductService>, IProductService
{
    private IConfiguration _configuration;

    public ProductService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<ProductService> logger, IMapper mapper,
        IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper,
        httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<GetProductDetailResponse>> GetAllProductPagingAsync(int page, int size,
        ProductFilter? filter, string? sortBy, bool isAsc)
    {
        var products = await _unitOfWork.GetRepository<Product>()
            .GetPagingListAsync(
                selector: p => new Product()
                {
                    Id = p.Id,
                    Description = p.Description,
                    Quantity = p.Quantity,
                    Name = p.Name,
                    Price = p.Price,
                    IsHidden = p.IsHidden,
                    CreatedAt = p.CreatedAt,
                    ModifiedAt = p.ModifiedAt,
                    ProductCategories =
                        p.ProductCategories.Any(pc => pc.ProductId == p.Id) ? p.ProductCategories : null,
                    ProductImages = p.ProductImages.Any(pi => pi.ProductId == p.Id) ? p.ProductImages : null,
                }, page: page, size: size,
                include: p => p.Include(p => p.ProductCategories).ThenInclude(pc => pc.Category), filter: filter,
                sortBy: sortBy, isAsc: isAsc);
        var response = _mapper.Map<IPaginate<GetProductDetailResponse>>(products);
        return response;
    }

    public async Task<GetProductDetailResponse> GetProductByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.ProductIdNotNull);
        var role = GetRoleFromJwt();
        var p = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(predicate: p => p.Id.Equals(id));
        if (p.IsHidden && role != RoleEnum.Manager && role != RoleEnum.Staff)
            throw new BadHttpRequestException(MessageConstant.Product.ProductIsHidden);
        var productResponse = new GetProductDetailResponse()
        {
            Id = p.Id,
            Name = p.Name,
            Description = p.Description,
            Price = p.Price,
            Quantity = p.Quantity,
            CreatedAt = p.CreatedAt,
            ModifiedAt = p.ModifiedAt,
            IsHidden = p.IsHidden,
            ProductImages =
                p.ProductImages.Select(pi =>
                        new ProductImageResponse() { Id = pi.Id, ImageUrl = pi.ImageUrl, IsMain = pi.IsMain })
                    .ToList(),
            Categories = p.ProductCategories.Select(pc => pc.Category)
                .Select(c => new CategoryResponse()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.ModifiedAt
                })
                .ToList()
        };
        return productResponse;
    }

    public async Task<GetProductResponse> CreateProductAsync(CreateProductRequest request)
    {
        var product = _mapper.Map<Product>(request);
        product.Id = Guid.NewGuid();
        product.CreatedAt = TimeUtil.GetCurrentSEATime();
        product.ModifiedAt = TimeUtil.GetCurrentSEATime();
        decimal expectedPrice = 0;
        if (request.CategoryIds != null)
        {
            foreach (var categoryId in request.CategoryIds)
            {
                var category = await _unitOfWork.GetRepository<Category>()
                    .SingleOrDefaultAsync(predicate: c => c.Id.Equals(categoryId));
                if (category == null) throw new BadHttpRequestException(MessageConstant.Category.CategoryNotFound);
            }
        }

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (request.CategoryIds != null)
                {
                    foreach (var categoryId in request.CategoryIds)
                    {
                        await _unitOfWork.GetRepository<ProductCategory>()
                            .InsertAsync(new ProductCategory() { ProductId = product.Id, CategoryId = categoryId });
                    }
                }

                // var mainImageUrl = await _firebaseService.UploadFileToFirebaseAsync(request.MainImage);
                // if (!string.IsNullOrEmpty(mainImageUrl))
                // {
                //     await _productImageRepository.InsertAsync(new ProductImage()
                //     {
                //         Id = Guid.NewGuid(),
                //         ProductId = product.Id,
                //         ImageUrl = mainImageUrl,
                //         IsMain = true
                //     });
                // }

                // if (request.SecondaryImages != null)
                // {
                //     var imageUrls = await _firebaseService.UploadFilesToFirebaseAsync(request.SecondaryImages);
                //     if (imageUrls.Any())
                //     {
                //         foreach (var imageUrl in imageUrls)
                //         {
                //             await _productImageRepository.InsertAsync(new ProductImage()
                //             {
                //                 Id = Guid.NewGuid(),
                //                 ProductId = product.Id,
                //                 ImageUrl = imageUrl,
                //                 IsMain = false
                //             });
                //         }
                //     }
                // }
                await _unitOfWork.GetRepository<Product>().InsertAsync(product);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess) return null;
                transaction.Complete();
                return _mapper.Map<GetProductResponse>(product);
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
    public async Task<GetProductResponse> UpdateProductByIdAsync(Guid id, UpdateProductRequest request)
    {
        if(id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.ProductIdNotNull);

        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: p => p.Id.Equals(id)
        );
        if(product == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
        product.Name = string.IsNullOrEmpty(request.Name) ? product.Name : request.Name;
        product.Description = string.IsNullOrEmpty(request.Description) ? product.Description : request.Description;
        // product.Price = (int) (request.Price == null ? product.Price : request.Price);
        product.Quantity = (int)(request.Quantity == null ? product.Quantity : request.Quantity);
        product.IsHidden = (bool) (request.IsHidden == null ? product.IsHidden : request.IsHidden);
        product.ModifiedAt = TimeUtil.GetCurrentSEATime();
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                product.Price = (int) (request.Price == null ? product.Price : request.Price);
                _unitOfWork.GetRepository<Product>().UpdateAsync(product);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                transactionScope.Complete();
                GetProductResponse productResponse = null;
                if (isSuccess)
                {
                    productResponse = _mapper.Map<GetProductResponse>(product);
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
                return productResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }
    // public async Task<IPaginate<GetProductResponse>> GetProductByCategoryIdAsync(Guid categoryId, int page, int size)
    // {
    //     if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Category.CategoryIdNotNull);
    //
    //     var products = await _unitOfWork.GetRepository<Product>().GetPagingListAsync(
    //         selector: p => new Product()
    //         {
    //             Id = p.Id,
    //             Description = p.Description,
    //             Quantity = p.Quantity,
    //             Name = p.Name,
    //             Price = p.Price,
    //             IsHidden = p.IsHidden,
    //             CreatedAt = p.CreatedAt,
    //             ModifiedAt = p.ModifiedAt,
    //             ProductCategories = p.ProductCategories.Any(pc => pc.ProductId == p.Id) ? p.ProductCategories : null,
    //             ProductImages = p.ProductImages.Any(pi => pi.ProductId == p.Id) ? p.ProductImages : null,
    //         },
    //         predicate: p => p.ProductCategories.Any(pc => pc.CategoryId == categoryId),
    //         page: page,
    //         size: size,
    //         include: p => p.Include(p => p.ProductImages),
    //         filter: null
    //     );
    //     var productResponses = _mapper.Map<IPaginate<GetProductResponse>>(products);
    //     return productResponses;
    // }
    
    // public async Task<GetProductResponse> DeleteProductImageById(Guid id)
    // {
    //     if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.ProductImage.ProductImageIdNotNull);
    //     var productImage = await _unitOfWork.GetRepository<ProductImage>().SingleOrDefaultAsync(
    //         predicate: pi => pi.Id == id,
    //         include: pi => pi.Include(pi => pi.Product)
    //     );
    //     if (productImage == null) throw new BadHttpRequestException(MessageConstant.ProductImage.ProductImageNotFound);
    //     _unitOfWork.GetRepository<ProductImage>().DeleteAsync(productImage);
    //
    //     bool isSuccess = await _unitOfWork.CommitAsync() > 0;
    //     GetProductResponse productResponse = null;
    //     if (isSuccess) productResponse = _mapper.Map<GetProductResponse>(productImage.Product);
    //     return productResponse;
    // }
    
    public async Task<GetProductResponse> UpdateProductImageByProductIdAsync(Guid productId,
        ICollection<ImageProductRequest> request)
    {
        if (productId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Product.ProductIdNotNull);
        var product = await _unitOfWork.GetRepository<Product>().SingleOrDefaultAsync(
            predicate: x => x.Id == productId,
            include: p => p.Include(p => p.ProductImages)
                .Include(p => p.ProductCategories)
                .ThenInclude(pc => pc.Category)
        );
        if (product == null) throw new BadHttpRequestException(MessageConstant.Product.ProductNotFound);
        
        if(request.Where(pi => pi.IsMain).ToList().Count != 1)
            throw new BadHttpRequestException(MessageConstant.ProductImage.WrongMainImageQuantity);
        var requestProductImagesId = request
            .Where(pi => pi.Id != null)
            .Select(pi => pi.Id!.Value).ToList();

        var ProductImageIds = await _unitOfWork.GetRepository<ProductImage>().GetListAsync(
            predicate: pi => pi.ProductId == productId && !requestProductImagesId.Contains(pi.Id)
        );
        var removedProductImageIds = ProductImageIds.Select(pi => pi.Id).ToList();
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                foreach (var removeProductImageId in removedProductImageIds)
                {
                    var productImage = await _unitOfWork.GetRepository<ProductImage>().SingleOrDefaultAsync(
                        predicate: pi => pi.Id == removeProductImageId,
                        include:pi => pi.Include(pi => pi.Product)
                    );
                    _unitOfWork.GetRepository<ProductImage>().DeleteAsync(productImage);
                }
                // foreach (var imageProduct in request)
                // {
                //     if (imageProduct.Id == null)
                //     {
                //         var imageUrl = await _firebaseService.UploadFileToFirebaseAsync(imageProduct.ImageUrl);
                //         if (string.IsNullOrEmpty(imageUrl))
                //             throw new BadHttpRequestException(MessageConstant.ProductImage.UploadImageFail);
                //         var newProductImage = new ProductImage()
                //         {
                //             Id = Guid.NewGuid(),
                //             IsMain = imageProduct.IsMain,
                //             ImageUrl = imageUrl,
                //             ProductId = product.Id
                //         };
                //         await _productImageRepository.InsertAsync(newProductImage);
                //     }
                //     else
                //     {
                //         var productImage = _mapper.Map<ProductImage>(imageProduct);
                //         productImage.Id = imageProduct.Id!.Value;
                //         productImage.ProductId = product.Id;
                //         _productImageRepository.UpdateAsync(productImage);
                //     }
                // }
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                transactionScope.Complete();
                GetProductResponse productResponse = null;
                if (isSuccess) productResponse = _mapper.Map<GetProductResponse>(product);
                return productResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }
    
}