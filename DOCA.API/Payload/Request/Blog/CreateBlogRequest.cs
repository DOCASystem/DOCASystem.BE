using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.Request.Blog;

public class CreateBlogRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool IsHidden { get; set; }
    
    public List<Guid>? BlogCategoryIds { get; set; }
    [Required]
    public string MainImage { get; set; }
    public List<string>? SecondaryImages { get; set; }
}