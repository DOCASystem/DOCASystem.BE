using DOCA.API.Enums;
using DOCA.API.Payload.Response.Animal;
using DOCA.API.Payload.Response.BlogCategory;

namespace DOCA.API.Payload.Response.Blog;

public class GetBlogDetailResponse
{
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public BlogEnum Status { get; set; }
    public bool IsHidden { get; set; }
    public virtual ICollection<BlogCategoryResponse>? BlogCategories { get; set; }
    public virtual ICollection<GetAnimalResponse>? Animals { get; set; }
}