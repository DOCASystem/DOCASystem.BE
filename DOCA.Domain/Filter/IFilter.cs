using System.Linq.Expressions;

namespace DOCA.Domain.Filter;

public interface IFilter<T>
{
    Expression<Func<T, bool>> ToExpression();
}