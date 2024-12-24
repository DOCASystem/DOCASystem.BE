using DOCA.Domain.Models;

namespace DOCA.API.Payload.Response.Animal;

public class GetAnimalDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public virtual ICollection<AnimalCategoryResponse>? AnimalCategories { get; set; }
    public virtual ICollection<AnimalImageResponse>? AnimalImage { get; set; }
}