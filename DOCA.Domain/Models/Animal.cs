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
    public Guid AnimalCategoryId { get; set; }
    [ForeignKey(nameof(AnimalCategoryId))]
    public AnimalCategory AnimalCategory { get; set; }
}