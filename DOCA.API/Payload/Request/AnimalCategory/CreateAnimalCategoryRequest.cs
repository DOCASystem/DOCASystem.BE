using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class CreateAnimalCategoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]  
    public string Description { get; set; }
}