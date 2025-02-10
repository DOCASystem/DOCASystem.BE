using System.Linq.Expressions;
using DOCA.API.Enums;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class StaffFilter: IFilter<Staff>
{
    public StaffType? Type { get; set; }
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public Expression<Func<Staff, bool>> ToExpression()
    {
        return staff =>
            (!Type.HasValue || staff.Type == Type) &&
            (string.IsNullOrEmpty(Username) || staff.User.Username.Contains(Username)) &&
            (string.IsNullOrEmpty(PhoneNumber) || staff.User.PhoneNumber.Contains(PhoneNumber));
    }
}