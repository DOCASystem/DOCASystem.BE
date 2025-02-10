using System.Linq.Expressions;
using DOCA.Domain.Models;

namespace DOCA.Domain.Filter;

public class MemberFilter: IFilter<Member>
{
    public string? Username { get; set; }
    public string? PhoneNumber { get; set; }
    public Expression<Func<Member, bool>> ToExpression()
    {
        return member =>
            (string.IsNullOrEmpty(Username) || member.User.Username.Contains(Username)) &&
            (string.IsNullOrEmpty(PhoneNumber) || member.User.PhoneNumber.Contains(PhoneNumber));
    }
}