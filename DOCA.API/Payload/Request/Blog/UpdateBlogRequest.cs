using DOCA.API.Enums;

namespace DOCA.API.Payload.Request.Blog;

public class UpdateBlogRequest
{
    public string? Name { get; set; }
    public string? Description { get; set; }
    public BlogEnum Status { get; set; }
    public bool? IsHidden { get; set; }
}