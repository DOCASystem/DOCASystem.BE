using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Animal;

public class DeleteImageAnimalRequest
{
    [Required]
    public Guid Id { get; set; }
}