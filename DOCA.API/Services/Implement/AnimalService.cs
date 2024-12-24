using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Enums;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.Product;
using DOCA.API.Services.Interface;
using DOCA.API.Utils;
using DOCA.Domain.Filter;
using DOCA.Domain.Models;
using DOCA.Domain.Paginate;
using DOCA.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DOCA.API.Services.Implement;

public class AnimalService : BaseService<AnimalService>, IAnimalService
{
    private IConfiguration _configuration;
    public AnimalService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<AnimalService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<GetAnimalDetailResponse>> GetAllAnimalPagingAsync(int page, int size, AnimalFilter? filter, string? sortBy, bool isAsc)
    {
        var animals = await _unitOfWork.GetRepository<Animal>()
            .GetPagingListAsync(
                selector: a => new Animal()
                {
                    Id = a.Id,
                    Name = a.Name,
                    Description = a.Description,
                    Age = a.Age,
                    Sex = a.Sex,
                    CreatedAt = a.CreatedAt,
                    ModifiedAt = a.ModifiedAt,
                    AnimalCategoryRelationship = 
                        a.AnimalCategoryRelationship.Any(acr => acr.AnimalId == a.Id) ? a.AnimalCategoryRelationship : null,
                    AnimalImage = a.AnimalImage.Any(ai => ai.AnimalId == a.Id) ? a.AnimalImage : null,
                }, page: page, size: size,
                include: a => a.Include(a => a.AnimalCategoryRelationship).ThenInclude(arc => arc.AnimalCategory), filter: filter,
                sortBy: sortBy, isAsc: isAsc);
        var response = _mapper.Map<IPaginate<GetAnimalDetailResponse>>(animals);
        return response;
    }

    public async Task<GetAnimalDetailResponse> GetAnimalByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);
        var role = GetRoleFromJwt();
        var a = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(predicate: a => a.Id.Equals(id));
        if ( role != RoleEnum.Manager && role != RoleEnum.Staff)
            throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
        var animalResponse = new GetAnimalDetailResponse()
        {
            Id = a.Id,
            Name = a.Name,
            Description = a.Description,
            Age = a.Age,
            Sex = a.Sex,
            CreatedAt = a.CreatedAt,
            ModifiedAt = a.ModifiedAt,
            AnimalImage =
                a.AnimalImage.Select(ai =>
                        new AnimalImageResponse() { Id = ai.Id, ImageUrl = ai.ImageUrl, IsMain = ai.IsMain })
                    .ToList(),
            AnimalCategories = a.AnimalCategoryRelationship.Select(ac => ac.AnimalCategory)
                .Select(c => new AnimalCategoryResponse()
                {
                    Id = c.Id,
                    Name = c.Name,
                    Description = c.Description,
                    CreatedAt = c.CreatedAt,
                    ModifiedAt = c.ModifiedAt
                })
                .ToList()
        };
        return animalResponse;
    }

    public async Task<GetAnimalResponse> CreateAnimalAsync(CreateAnimalRequest request)
    {
        var animal = _mapper.Map<Animal>(request);
        animal.Id = Guid.NewGuid();
        animal.CreatedAt = TimeUtil.GetCurrentSEATime();
        animal.ModifiedAt = TimeUtil.GetCurrentSEATime();
        decimal expectedPrice = 0;
        if (request.AnimalcategoryIds != null)
        {
            foreach (var animalcategoryId in request.AnimalcategoryIds)
            {
                var category = await _unitOfWork.GetRepository<AnimalCategory>()
                    .SingleOrDefaultAsync(predicate: c => c.Id.Equals(animalcategoryId));
                if (category == null) throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
            }
        }

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (request.AnimalcategoryIds != null)
                {
                    foreach (var animalcategoryId in request.AnimalcategoryIds)
                    {
                        await _unitOfWork.GetRepository<AnimalCategoryRelationship>()
                            .InsertAsync(new AnimalCategoryRelationship() { AnimalId = animal.Id, AnimalCategoryId = animalcategoryId });
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
                await _unitOfWork.GetRepository<Animal>().InsertAsync(animal);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                if (!isSuccess) return null;
                transaction.Complete();
                return _mapper.Map<GetAnimalResponse>(animal);
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

    public async Task<GetAnimalResponse> UpdateAnimalByIdAsync(Guid id, UpdateAnimalRequest request)
    {
        if(id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);

        var animal = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
            predicate: a => a.Id.Equals(id)
        );
        if(animal == null) throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
        animal.Name = string.IsNullOrEmpty(request.Name) ? animal.Name : request.Name;
        animal.Description = string.IsNullOrEmpty(request.Description) ? animal.Description : request.Description;
        animal.Age = (int)(request.Age == null ? animal.Age : request.Age);
        animal.Sex = string.IsNullOrEmpty(request.Sex) ? animal.Sex : request.Sex;
        animal.ModifiedAt = TimeUtil.GetCurrentSEATime();
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                _unitOfWork.GetRepository<Animal>().UpdateAsync(animal);
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                transactionScope.Complete();
                GetAnimalResponse animalResponse = null;
                if (isSuccess)
                {
                    animalResponse = _mapper.Map<GetAnimalResponse>(animal);
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
                return animalResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }

    public async Task<IPaginate<GetAnimalResponse>> GetAnimalByAnimalCategoryIdAsync(Guid categoryId, int page, int size)
    {
        if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);
    
        var animals = await _unitOfWork.GetRepository<Animal>().GetPagingListAsync(
            selector: a => new Animal()
            {
                Id = a.Id,
                Description = a.Description,
                Age = a.Age,
                Name = a.Name,
                Sex = a.Sex,
                CreatedAt = a.CreatedAt,
                ModifiedAt = a.ModifiedAt,
                AnimalCategoryRelationship = a.AnimalCategoryRelationship.Any(acr => acr.AnimalId == a.Id) ? a.AnimalCategoryRelationship : null,
                AnimalImage = a.AnimalImage.Any(ai => ai.AnimalId == a.Id) ? a.AnimalImage : null,
            },
            predicate: a => a.AnimalCategoryRelationship.Any(pc => pc.AnimalCategoryId == categoryId),
            page: page,
            size: size,
            include: a => a.Include(a => a.AnimalImage),
            filter: null
        );
        var productResponses = _mapper.Map<IPaginate<GetAnimalResponse>>(animals);
        return productResponses;
    }

    public async Task<GetAnimalResponse> UpdateAnimalImageByAnimalIdAsync(Guid animalId, ICollection<ImageAnimalRequest> request)
    {
        if (animalId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);
        var animal = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
            predicate: x => x.Id == animalId,
            include: a => a.Include(a => a.AnimalImage)
                .Include(a => a.AnimalCategoryRelationship)
                .ThenInclude(ac => ac.AnimalCategory)
        );
        if (animal == null) throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
        
        if(request.Where(ai => ai.IsMain).ToList().Count != 1)
            throw new BadHttpRequestException(MessageConstant.AnimalImage.WrongMainImageQuantity);
        var requestAnimalImagesId = request
            .Where(ai => ai.Id != null)
            .Select(ai => ai.Id!.Value).ToList();

        var AnimalImageIds = await _unitOfWork.GetRepository<AnimalImage>().GetListAsync(
            predicate: ai => ai.AnimalId == animalId && !requestAnimalImagesId.Contains(ai.Id)
        );
        var removedAnimalImageIds = AnimalImageIds.Select(ai => ai.Id).ToList();
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                foreach (var removedAnimalImageId in removedAnimalImageIds)
                {
                    var animalImage = await _unitOfWork.GetRepository<AnimalImage>().SingleOrDefaultAsync(
                        predicate: ai => ai.Id == removedAnimalImageId,
                        include:ai => ai.Include(ai => ai.Animal)
                    );
                    _unitOfWork.GetRepository<AnimalImage>().DeleteAsync(animalImage);
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
                GetAnimalResponse animalResponse = null;
                if (isSuccess) animalResponse = _mapper.Map<GetAnimalResponse>(animal);
                return animalResponse;
            }
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                return null;
            }
        }
    }
}