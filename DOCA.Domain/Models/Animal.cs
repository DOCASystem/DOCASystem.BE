using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class Animal
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public int Age { get; set; }
    public string Sex { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public virtual ICollection<AnimalCategoryRelationship>? AnimalCategoryRelationship { get; set; }
    
    public virtual ICollection<AnimalImage>? AnimalImage { get; set; }
}