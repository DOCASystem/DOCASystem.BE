using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Category;

public class UpdateProductCategoryRequest
{
    [Required]
    public List<Guid> ProductIds { get; set; }
}