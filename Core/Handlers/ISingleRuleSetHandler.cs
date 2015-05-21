using System;
using System.Linq.Expressions;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Handlers
{
    public interface ISingleRuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        Expression<Func<T, bool>> Rule { get; set; }
    }
}