using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class AnimalCategory
{
    [Key]
    public Guid Id { get; set; }
    public string AnimalName { get; set; }
}