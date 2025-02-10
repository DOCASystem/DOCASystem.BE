using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using DOCA.API.Enums;

namespace DOCA.Domain.Models;

public class Staff
{
    [Key]
    public Guid Id { get; set; }
    public StaffType Type { get; set; }
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
}