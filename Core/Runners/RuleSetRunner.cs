using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Runners
{
    public sealed class RuleSetRunner
    {
        private static readonly Lazy<RuleSetRunner> Lazy = new Lazy<RuleSetRunner>(() => new RuleSetRunner());
        
        public IDictionary<Type, object> Rules { get; private set; }

        public void AddRule<T>(object ruleSetObject)
        {
            if (!Rules.ContainsKey(typeof(T)))
                Rules.Add(typeof(T),ruleSetObject);
        }

        public IEnumerable<IRuleSetHandler<T>> GetRules<T>() where T : IRuleRequest,new()
        {
            var found = Rules.Where(x => x.Key == typeof(T));

            var result = found.Select(y => y.Value).Cast<IRuleSetHandler<T>>();
            return result;
        }

        public static RuleSetRunner Instance { get { return Lazy.Value; } }

        private RuleSetRunner()
        {
            Rules = new ConcurrentDictionary<Type, object>();
        }

        public void VerifyRule(IRuleRequest ruleRequest = null)
        {
            foreach (var action in Rules.Where(rule => rule.Key == ruleRequest.GetType()).Select(rule => rule.Value).OfType<IRuleRunner>())
            {
                action.VerifyRule(ruleRequest);
            }
        }

    }
}
