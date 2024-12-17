using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class Category
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(255)]
    public string Name { get; set; }
    [MaxLength(1000)]
    public string? Description { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
}