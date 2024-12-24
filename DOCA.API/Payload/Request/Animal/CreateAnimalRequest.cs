using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class CreateAnimalRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public int Age { get; set; }
    [Required]
    public string Sex { get; set; }
    public List<Guid>? AnimalcategoryIds { get; set; }
    [Required]
    public string MainImage { get; set; }
    public List<string>? SecondImage { get; set; }
    
}