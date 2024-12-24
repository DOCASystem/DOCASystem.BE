using System.Linq.Expressions;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class BlogCategoryFilter: IFilter<BlogCategory>
{
    public string? Name { get; set; }
    public Expression<Func<BlogCategory, bool>> ToExpression()
    {
        return blogCategory => 
            (string.IsNullOrEmpty(Name) || blogCategory.Name.Contains(Name));
    }
}