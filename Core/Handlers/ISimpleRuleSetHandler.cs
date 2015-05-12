using System;
using System.Linq.Expressions;

namespace WebMinder.Core.Handlers
{
    public interface ISimpleRuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        Expression<Func<T, bool>> Rule { get; set; }
    }
}