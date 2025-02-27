using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class AddImageAnimalRequest
{
    [Required]
    public IFormFile ImageUrl { get; set; }
    [Required]
    public bool IsMain { get; set; }
}