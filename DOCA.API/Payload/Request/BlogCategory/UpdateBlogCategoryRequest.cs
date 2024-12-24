using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.BlogCategory;

public class UpdateBlogCategoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]  
    public string Description { get; set; }
}