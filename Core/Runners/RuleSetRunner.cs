using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Runners
{
    public class RuleSetRunner
    {
        protected static readonly Lazy<RuleSetRunner> Lazy = new Lazy<RuleSetRunner>(() => new RuleSetRunner());
        
        public List<KeyValuePair<Type, object>> Rules { get; private set; }

        public void AddRule<T>(object ruleSetObject)
        {
            if (!Rules.Exists(a=>ruleSetObject.GetType().IsAssignableFrom(a.Key)))
                Rules.Add(new KeyValuePair<Type, object>(typeof(T),ruleSetObject));
        }

        public void AddRule(Type ruleRequestType,object ruleSetObject)
        {
            if (!Rules.Exists(a=>a.Key == ruleRequestType))
                Rules.Add(new KeyValuePair<Type, object>(ruleRequestType, ruleSetObject));
        }

        public IEnumerable<IRuleSetHandler<T>> GetRules<T>() where T : IRuleRequest,new()
        {
            var found = Rules.Where(x => x.Key == typeof(T));

            var result = found.Where(y => y.Value is IRuleSetHandler<T>)
                .Select(z=>z.Value)
                .Cast<IRuleSetHandler<T>>();
            return result;
        }

        public RuleSetRunner()
        {
            Rules = new List<KeyValuePair<Type, object>>();
        }

        public void VerifyRule(IRuleRequest ruleRequest = null)
        {
            foreach (var action in RulesInContainer(ruleRequest))
            {
                action.VerifyRule(ruleRequest);
            }
        }

        protected IEnumerable<IRuleRunner> RulesInContainer(IRuleRequest ruleRequest)
        {
            if (ruleRequest == null) return null;
            var  found = Rules.Where(x => x.Key == ruleRequest.GetType());

            var result = found
                .Select(z => z.Value)
                .Cast<IRuleRunner>();

            return result;

        }

        public void VerifyAllRules()
        {
            Instance.Rules
                .Select(r=>r.Value as IRuleRunner)
                .ToList()
                .ForEach(rule => rule.VerifyRule());
        }

        public static RuleSetRunner Instance { get { return Lazy.Value; } }
    }
}
