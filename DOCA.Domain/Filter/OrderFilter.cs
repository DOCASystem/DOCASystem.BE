using System.Linq.Expressions;
using DOCA.API.Enums;
using DOCA.Domain.Models;
using Microsoft.IdentityModel.Tokens;

namespace DOCA.Domain.Filter;

public class OrderFilter : IFilter<Order>
{
    public OrderStatus? Status { get; set; }
    public DateTime? CreatedAt { get; set; }
    public string? Username { get; set; }
    
    public string? Code { get; set; }
    
    public Expression<Func<Order, bool>> ToExpression()
    {
        return order =>
            (!Status.HasValue || order.Status == Status) &&
            (!CreatedAt.HasValue || order.CreatedAt == CreatedAt) &&
            (string.IsNullOrEmpty(Username) || order.Member.User.Username.Contains(Username)) &&
            (string.IsNullOrEmpty(Code) || order.Code.Contains(Code));
    }
}