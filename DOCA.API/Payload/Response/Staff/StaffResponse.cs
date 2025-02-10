using DOCA.API.Enums;

namespace DOCA.API.Payload.Response.Staff;

public class StaffResponse
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string Username { get; set; }
    public string PhoneNumber { get; set; }
    public string FullName { get; set; }
    public StaffType Type { get; set; }
}