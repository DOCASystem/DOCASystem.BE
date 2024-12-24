using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class UpdateAnimalCategoryRelationshipRequest
{
    [Required]
    public List<Guid> AnimalIds { get; set; }
}