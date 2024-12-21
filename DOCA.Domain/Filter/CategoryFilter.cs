using System.Linq.Expressions;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class CategoryFilter: IFilter<Category>
{
    public string? Name { get; set; }
    public Expression<Func<Category, bool>> ToExpression()
    {
        return category => 
            (string.IsNullOrEmpty(Name) || category.Name.Contains(Name));
    }
}