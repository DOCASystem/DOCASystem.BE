using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class AnimalImage
{
    [Key]
    public Guid Id { get; set; }
    public Guid AnimalId { get; set; }
    [ForeignKey(nameof(AnimalId))]
    public Animal Animal;
    public string ImageUrl { get; set; }
    public bool IsMain { get; set; }
}