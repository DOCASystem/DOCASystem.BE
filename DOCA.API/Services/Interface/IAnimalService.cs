using DOCA.API.Payload.Request.Animal;
using DOCA.API.Payload.Response.Animal;
using DOCA.Domain.Filter;
using DOCA.Domain.Paginate;

namespace DOCA.API.Services.Interface;

public interface IAnimalService
{
    Task<IPaginate<GetAnimalDetailResponse>> GetAllAnimalPagingAsync(int page, int size, AnimalFilter? filter,
        string? sortBy, bool isAsc);

    Task<GetAnimalDetailResponse> GetAnimalByIdAsync(Guid id);

    Task<GetAnimalResponse> CreateAnimalAsync(CreateAnimalRequest request);

    Task<GetAnimalResponse> UpdateAnimalByIdAsync(Guid id, UpdateAnimalRequest request);

    Task<IPaginate<GetAnimalResponse>> GetAnimalByAnimalCategoryIdAsync(Guid categoryId, int page, int size);

    // Task<GetAnimalResponse> DeleteAnimalImageById(Guid id);

    Task<GetAnimalResponse> UpdateAnimalImageByAnimalIdAsync(Guid animalId, ICollection<ImageAnimalRequest> request);
}