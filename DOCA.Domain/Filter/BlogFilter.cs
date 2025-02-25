using System.Linq.Expressions;
using DOCA.API.Enums;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class BlogFilter : IFilter<Blog>
{
    public string? Name { get; set; }
    public DateTime? CreateAt { get; set; }
    public List<Guid>? CategoryIds { get; set; }
    public BlogEnum? Status { get; set; } 

    public Expression<Func<Blog, bool>> ToExpression()
    {
        return blog =>
            (string.IsNullOrEmpty(Name) || blog.Name.Contains(Name)) &&
            (!CreateAt.HasValue || blog.CreatedAt == CreateAt) &&
            (CategoryIds == null || blog.BlogCategoryRelationship!.Any(pc => CategoryIds.Contains(pc.BlogCategoryId))) &&
            (!Status.HasValue || blog.Status == Status);
    }
}
