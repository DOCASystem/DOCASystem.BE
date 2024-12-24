using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class BlogCategoryRelationship
{
    [Key]
    public Guid Id { get; set; }
    public Guid BlogId { get; set; }
    [ForeignKey(nameof(BlogId))]
    public Blog BLog { get; set; }
    public Guid BlogCategoryId { get; set; }
    [ForeignKey(nameof(BlogCategoryId))]
    public BlogCategory BlogCategory { get; set; }
}