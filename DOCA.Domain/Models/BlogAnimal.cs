using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class BlogAnimal
{
    [Key]
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    [ForeignKey(nameof(BlogId))]
    public Blog Blog { get; set; }
    public Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public Animal Animal { get; set; }
}