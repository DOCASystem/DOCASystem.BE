using System.Linq.Expressions;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class AnimalCategoryFilter : IFilter<AnimalCategory>
{
    public string? Name { get; set; }
    public Expression<Func<AnimalCategory, bool>> ToExpression()
    {
        return animalCategory => 
            (string.IsNullOrEmpty(Name) || animalCategory.Name.Contains(Name));
    }
}