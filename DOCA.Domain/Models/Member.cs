using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOCA.Domain.Models;

public class Member
{
    
    public Guid UserId { get; set; }
    [ForeignKey(nameof(UserId))]
    public User User { get; set; }
    [MaxLength(255)]
    public string? Province { get; set; }
    public string? ProvinceCode { get; set; }
    [MaxLength(255)]
    public string? District { get; set; }
    public string? DistrictCode { get; set; }
    [MaxLength(255)]
    public string? Commune { get; set; }
    public string? CommuneCode { get; set; }
    [MaxLength(500)]
    public string? Address { get; set; }
}