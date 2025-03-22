using System.ComponentModel.DataAnnotations;
using DOCA.API.Enums;

namespace DOCA.API.Payload.Request.Blog;

public class CreateBlogRequest
{
    [Required]
    public string Name { get; set; }
    [Required]
    public string Description { get; set; }
    [Required]
    public bool IsHindden { get; set; }
    [Required]
    public BlogEnum Status { get; set; }
    public List<Guid>? BlogCategoryIds { get; set; }
    public List<Guid>? AnimalIds { get; set; }
    // [Required]
    // public IFormFile MainImage { get; set; }
    // public List<IFormFile>? SecondaryImages { get; set; }
}