using DOCA.API.Enums;

namespace DOCA.API.Payload.Response.Blog;

public class GetBlogResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public BlogEnum Status { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsHindden { get; set; }
}