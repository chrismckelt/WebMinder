using System;
using System.Collections.Generic;
using System.Linq;

namespace WebMinder.Core
{
    public sealed class RuleSetRunner
    {
        private static readonly Lazy<RuleSetRunner> Lazy = new Lazy<RuleSetRunner>(() => new RuleSetRunner());
        private IList<object> _rules;

        public IEnumerable<IRuleSetHandler<IRuleRequest>> Rules
        {
            get
            {
                return _rules.Cast<IRuleSetHandler<IRuleRequest>>().AsEnumerable();
            }
        }

        public void AddRule<T>(RequestAnalyserRuleSet<T> ruleSet) where T : IRuleRequest
        {
            _rules.Add((ruleSet));
        }

        public static RuleSetRunner Instance { get { return Lazy.Value; } }

        private RuleSetRunner()
        {
            _rules = new List<object>();
        }

        public void Run(IRuleRequest rr)
        {
            foreach (var rule in Rules)
            {
                rule.Run(rr);
            }
        }
    }
}
