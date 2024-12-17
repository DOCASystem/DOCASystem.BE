using System.ComponentModel.DataAnnotations;

namespace DOCA.Domain.Models;

public class Blog
{
    [Key]
    public Guid Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int Quantity { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ModifiedAt { get; set; }
    public bool IsHindden { get; set; }
}