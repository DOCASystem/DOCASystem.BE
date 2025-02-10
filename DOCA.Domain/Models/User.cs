using System.ComponentModel.DataAnnotations;
using DOCA.API.Enums;


namespace DOCA.Domain.Models;

public class User 
{
    [Key]
    public Guid Id { get; set; }
    [MaxLength(250)]
    public string Username { get; set; }
    [MaxLength(250)]
    public string Password { get; set; }
    [MaxLength(13)]
    public string PhoneNumber { get; set; }
    [MaxLength(250)]
    public string FullName { get; set; }
    [MaxLength(250)]
    public RoleEnum Role { get; set; }
}