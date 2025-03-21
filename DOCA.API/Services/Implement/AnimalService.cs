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
    private IUploadService _uploadService;
    private IRedisService _redisService;
    public AnimalService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<AnimalService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, IRedisService redisService, IUploadService uploadService) : base(unitOfWork, logger, mapper, httpContextAccessor, configuration)
    {
        _configuration = configuration;
        _uploadService = uploadService;
        _redisService = redisService;
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
        var a = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
            predicate: a => a.Id == id,
            include: source => source
                .Include(p => p.AnimalImage)
                .Include(p => p.AnimalCategoryRelationship).ThenInclude(pc => pc.AnimalCategory)
            );
        // if ( role != RoleEnum.Manager && role != RoleEnum.Staff)
        //     throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
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
            foreach (var categoryId in request.AnimalcategoryIds)
            {
                var category = await _unitOfWork.GetRepository<AnimalCategory>()
                    .SingleOrDefaultAsync(predicate: c => c.Id.Equals(categoryId));
                if (category == null) throw new BadHttpRequestException(MessageConstant.Category.CategoryNotFound);
            }
        }

        using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (request.AnimalcategoryIds != null)
                {
                    foreach (var categoryId in request.AnimalcategoryIds)
                    {
                        await _unitOfWork.GetRepository<AnimalCategoryRelationship>()
                            .InsertAsync(new AnimalCategoryRelationship() { AnimalId = animal.Id, AnimalCategoryId = categoryId });
                    }
                }

                var mainImageUrl = await _uploadService.UploadImageAsync(request.MainImage);
                if (!string.IsNullOrEmpty(mainImageUrl))
                {
                    await _unitOfWork.GetRepository<AnimalImage>().InsertAsync(new AnimalImage()
                    {
                        Id = Guid.NewGuid(),
                        AnimalId = animal.Id,
                        ImageUrl = mainImageUrl,
                        IsMain = true
                    });
                }

                if (request.SecondaryImages != null)
                {
                    var imageUrls = await _uploadService.UploadImageAsync(request.SecondaryImages);
                    if (imageUrls.Any())
                    {
                        foreach (var imageUrl in imageUrls)
                        {
                            await _unitOfWork.GetRepository<AnimalImage>().InsertAsync(new AnimalImage()
                            {
                                Id = Guid.NewGuid(),
                                AnimalId = animal.Id,
                                ImageUrl = imageUrl,
                                IsMain = false
                            });
                        }
                    }
                }
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
        var responses = _mapper.Map<IPaginate<GetAnimalResponse>>(animals);
        return responses;
    }

    public async Task<GetAnimalResponse> AddAnimalImageByAnimalIdAsync(Guid animalId, ICollection<AddImageAnimalRequest> request)
    {
        if (animalId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);
        var animal = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
            predicate: x => x.Id == animalId,
            include: a => a.Include(a => a.AnimalImage)
                .Include(a => a.AnimalCategoryRelationship)
                .ThenInclude(ac => ac.AnimalCategory)
        );
        if (animal == null) throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
        
        if(request.Where(ai => ai.IsMain).ToList().Count != 1 && animal.AnimalImage == null)
            throw new BadHttpRequestException(MessageConstant.AnimalImage.WrongMainImageQuantity);
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                foreach (var imageAnimal in request)
                {
                    var imageUrl = await _uploadService.UploadImageAsync(imageAnimal.ImageUrl);
                    if (string.IsNullOrEmpty(imageUrl))
                        throw new BadHttpRequestException(MessageConstant.AnimalImage.UploadImageFail);
                    var newAnimalImage = new AnimalImage()
                    {
                        Id = Guid.NewGuid(),
                        IsMain = imageAnimal.IsMain,
                        ImageUrl = imageUrl,
                        AnimalId = animal.Id
                    };
                    if (newAnimalImage.IsMain)
                    {
                        var imageMainOld = await _unitOfWork.GetRepository<AnimalImage>().SingleOrDefaultAsync(
                            predicate: a => a.IsMain == true && a.AnimalId == animal.Id
                        );

                        if (imageMainOld != null) 
                        {
                            imageMainOld.IsMain = false;
                            _unitOfWork.GetRepository<AnimalImage>().UpdateAsync(imageMainOld);
                        }
                    }
                    await _unitOfWork.GetRepository<AnimalImage>().InsertAsync(newAnimalImage);
                    var animalImage = _mapper.Map<AnimalImage>(imageAnimal);
                    animalImage.AnimalId = animal.Id;
                    _unitOfWork.GetRepository<AnimalImage>().UpdateAsync(animalImage);    
                }
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
    
    public async Task<GetAnimalResponse> DeleteAnimalImageByAnimalIdAsync(Guid animalId, ICollection<DeleteImageAnimalRequest> request)
    {
        if (animalId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.Animal.AnimalIdNotNull);
        var animal = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
            predicate: x => x.Id == animalId,
            include: a => a.Include(a => a.AnimalImage)
                .Include(a => a.AnimalCategoryRelationship)
                .ThenInclude(ac => ac.AnimalCategory)
        );
        if (animal == null) throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
        using (var transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                foreach (var imageAnimal in request)
                {
                    if(imageAnimal.Id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.AnimalImage.AnimalImageIdNotNull);
                    var image = await _unitOfWork.GetRepository<AnimalImage>().SingleOrDefaultAsync(
                        predicate: a => a.Id == imageAnimal.Id && a.AnimalId == animal.Id
                    );
                    if (image == null)
                    {
                        throw new BadHttpRequestException(MessageConstant.AnimalImage.AnimalImageNotFound);
                    }
                    if (image.IsMain)
                    {
                        throw new BadHttpRequestException(MessageConstant.AnimalImage.DeleteAnimalImageFail);
                    } 
                    _unitOfWork.GetRepository<AnimalImage>().DeleteAsync(image);  
                }
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