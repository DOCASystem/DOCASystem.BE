using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class AnimalCategory
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    
    public virtual ICollection<AnimalCategoryRelationship>? AnimalCategoryRelationship { get; set; }
}