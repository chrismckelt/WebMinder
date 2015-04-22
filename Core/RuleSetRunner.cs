using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace WebMinder.Core
{
    public sealed class RuleSetRunner
    {
        private static readonly Lazy<RuleSetRunner> Lazy = new Lazy<RuleSetRunner>(() => new RuleSetRunner());
        private IDictionary<Type, object> _rules;

        public void AddRule<T>(dynamic ruleSetObject) where T : IRuleRequest
        {
            _rules.Add(typeof(T),ruleSetObject);
        }

        public static RuleSetRunner Instance { get { return Lazy.Value; } }

        private RuleSetRunner()
        {
            _rules = new ConcurrentDictionary<Type, object>();
        }

        public void Run(IRuleRequest rr, bool addRequestToItemsCollection = true)
        {
            foreach (var action in _rules.Where(rule => rule.Key == rr.GetType()).Select(rule => rule.Value).OfType<IRuleRunner>())
            {
                action.Run(rr, addRequestToItemsCollection);
            }
        }
    }
}
