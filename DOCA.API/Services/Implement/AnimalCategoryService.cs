using System.Transactions;
using AutoMapper;
using DOCA.API.Constants;
using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Request.Category;
using DOCA.API.Payload.Response.Animal;
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

public class AnimalCategoryService : BaseService<AnimalCategoryService>, IAnimalCategoryService
{
    private IConfiguration _configuration;
    public AnimalCategoryService(IUnitOfWork<DOCADbContext> unitOfWork, ILogger<AnimalCategoryService> logger, IMapper mapper, IHttpContextAccessor httpContextAccessor, IConfiguration configuration) : base(unitOfWork, logger, mapper, httpContextAccessor)
    {
        _configuration = configuration;
    }

    public async Task<IPaginate<AnimalCategoryResponse>> GetAnimalCategoriesPagingAsync(int page, int size, AnimalCategoryFilter? filter)
    {
        var categories = await _unitOfWork.GetRepository<AnimalCategory>().GetPagingListAsync(
            page: page,
            size: size,
            selector: c => new AnimalCategory()
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
        var response = _mapper.Map<IPaginate<AnimalCategoryResponse>>(categories);
        return response;
    }

    public async Task<AnimalCategoryResponse> GetAnimalCategoryByIdAsync(Guid id)
    {
        if (id == Guid.Empty) throw new BadHttpRequestException(MessageConstant.AnimalCategory.AnimalCategoryIdNotNull);

        var category = await _unitOfWork.GetRepository<AnimalCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == id,
            include: c => c.Include(c => c.AnimalCategoryRelationship)
                .ThenInclude(c => c.Animal)
        );
        var categoryResponse = _mapper.Map<AnimalCategoryResponse>(category);
        return categoryResponse;
    }

    public async Task<AnimalCategoryResponse> UpdateAnimalCategoryByCategoryIdAsync(Guid categoryId, UpdateAnimalCategoryRelationshipRequest request)
    {
        if(categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.AnimalCategory.AnimalCategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<AnimalCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.AnimalCategoryRelationship)
                .ThenInclude(c => c.Animal)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.AnimalCategory.AnimalCategoryNotFound);

        var animalCategories = await _unitOfWork.GetRepository<AnimalCategoryRelationship>().GetListAsync(
            predicate: pc => pc.AnimalCategoryId == categoryId
        );
        var animalIds = animalCategories.Select(pc => pc.AnimalId).ToList();    
        var newAnimalIds = request.AnimalIds.Except(animalIds).ToList();
        var removeAnimalIds = animalIds.Except(request.AnimalIds).ToList();
        foreach (var newAnimalId in newAnimalIds)
        {
            var newAnimal = await _unitOfWork.GetRepository<Animal>().SingleOrDefaultAsync(
                predicate:x => x.Id == newAnimalId,
                include: p => p.Include(p => p.AnimalImage)
                    .Include(p => p.AnimalCategoryRelationship)
                    .ThenInclude(pc => pc.Animal)
            );
            if (newAnimal == null)
            {
                throw new BadHttpRequestException(MessageConstant.Animal.AnimalNotFound);
            }
        }
        if(!removeAnimalIds.Any() && !newAnimalIds.Any()) return _mapper.Map<AnimalCategoryResponse>(category);
        using (var transaction  = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
        {
            try
            {
                if (removeAnimalIds.Any())
                {
                    var removeAnimalCategories = await _unitOfWork.GetRepository<AnimalCategoryRelationship>().GetListAsync(
                        predicate: pc => animalIds.Contains(pc.AnimalId)
                    );
                    foreach (var removeAnimalCategorie in removeAnimalCategories)
                    {
                        _unitOfWork.GetRepository<AnimalCategoryRelationship>().DeleteAsync(removeAnimalCategorie);
                    }
                }
                
                if (newAnimalIds.Any())
                {
                    foreach (var newAnimalId in newAnimalIds)
                    {
                        await _unitOfWork.GetRepository<AnimalCategoryRelationship>().InsertAsync(
                            new AnimalCategoryRelationship()
                            {
                                AnimalId = newAnimalId,
                                AnimalCategoryId = categoryId
                            });
                    }
                }
                
                bool isSuccess = await _unitOfWork.CommitAsync() > 0;
                AnimalCategoryResponse response = null;
                transaction.Complete();
                if(isSuccess) response = _mapper.Map<AnimalCategoryResponse>(category);
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

    public async Task<AnimalCategoryResponse> UpdateAnimalCategoryAsync(Guid categoryId, UpdateAnimalCategoryRequest request)
    {
        if (categoryId == Guid.Empty) throw new BadHttpRequestException(MessageConstant.AnimalCategory.AnimalCategoryIdNotNull);
        var category = await _unitOfWork.GetRepository<AnimalCategory>().SingleOrDefaultAsync(
            predicate: c => c.Id == categoryId,
            include: c => c.Include(c => c.AnimalCategoryRelationship)
                .ThenInclude(c => c.Animal)
        );
        if (category == null) throw new BadHttpRequestException(MessageConstant.AnimalCategory.AnimalCategoryNotFound);
        category.Name = request.Name;
        category.Description = request.Description;
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();
        
        _unitOfWork.GetRepository<AnimalCategory>().UpdateAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        AnimalCategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<AnimalCategoryResponse>(category);
        return response;
    }

    public async Task<AnimalCategoryResponse> CreateAnimalCategoryAsync(CreateAnimalCategoryRequest request)
    {
        var category = _mapper.Map<AnimalCategory>(request);
        category.Id = Guid.NewGuid();
        category.CreatedAt = TimeUtil.GetCurrentSEATime();
        category.ModifiedAt = TimeUtil.GetCurrentSEATime();

        await _unitOfWork.GetRepository<AnimalCategory>().InsertAsync(category);
        var isSuccess = await _unitOfWork.CommitAsync() > 0;
        AnimalCategoryResponse response = null;
        if (isSuccess) response = _mapper.Map<AnimalCategoryResponse>(category);
        return response;
    }
}