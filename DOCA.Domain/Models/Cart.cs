using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class Cart
{
    [Key]
    public Guid Id { get; set; }

    public Guid MemberID { get; set; }
    [ForeignKey(nameof(MemberID))]
    public Member Member { get; set; }
}