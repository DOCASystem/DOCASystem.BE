using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Product;

public class DeleteImageProductRequest
{
    [Required]
    public Guid Id { get; set; }
}