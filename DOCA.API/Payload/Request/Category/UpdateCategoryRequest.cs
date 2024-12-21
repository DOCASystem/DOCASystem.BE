using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Response.Category;

public class UpdateCategoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
}