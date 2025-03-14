using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class AnimalCategoryRelationship
{
    [Key]
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public Animal Animal { get; set; }
    public Guid AnimalCategoryId { get; set; }
    [ForeignKey(nameof(AnimalCategoryId))]    
    public AnimalCategory AnimalCategory { get; set; }
}