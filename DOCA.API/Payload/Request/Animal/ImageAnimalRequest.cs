using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class ImageAnimalRequest
{
    public Guid? Id { get; set; }
    [Required]
    public string ImageUrl { get; set; }
    [Required]
    public bool IsMain { get; set; }
}