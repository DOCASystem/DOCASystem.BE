using System.ComponentModel.DataAnnotations;

namespace DOCA.API.Payload.BlogCategory;

public class UpdateBlogCategoryRelationshipRequest
{
    [Required]
    public List<Guid> BlogIds { get; set; }
}