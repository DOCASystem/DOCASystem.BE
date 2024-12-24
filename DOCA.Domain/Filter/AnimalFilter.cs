using System.Linq.Expressions;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class AnimalFilter : IFilter<Animal>
{
    public string? Name { get; set; }
    public DateTime? CreateAt { get; set; }
    public List<Guid>? AnimalCategoryIds { get; set; }
    public int? Age { get; set; }
    public string? Sex { get; set; }

    public Expression<Func<Animal, bool>> ToExpression()
    {
        return animal =>
            (string.IsNullOrEmpty(Name) || animal.Name.Contains(Name)) &&
            (!CreateAt.HasValue || animal.CreatedAt.Date == CreateAt.Value.Date) &&
            (AnimalCategoryIds == null || animal.AnimalCategoryRelationship!.Any(pc => AnimalCategoryIds.Contains(pc.AnimalCategoryId))) &&
            (!Age.HasValue || animal.Age == Age.Value) &&
            (string.IsNullOrEmpty(Sex) || animal.Sex.Equals(Sex, StringComparison.OrdinalIgnoreCase));
    }
}