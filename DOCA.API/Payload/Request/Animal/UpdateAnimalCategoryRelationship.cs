using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class UpdateAnimalCategoryRelationship
{
    [Required]
    public List<Guid> AnimalIds { get; set; }
}