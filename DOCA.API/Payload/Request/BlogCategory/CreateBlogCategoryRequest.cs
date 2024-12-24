using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.BlogCategory;

public class CreateBlogCategoryRequest
{
    [Required]
    public string Name { get; set; }
    [Required]  
    public string Description { get; set; }
}