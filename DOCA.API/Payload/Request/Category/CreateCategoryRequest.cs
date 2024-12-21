using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Category;

public class CreateCategoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]  
    public string Description { get; set; }
}