using System;
using System.Collections.Generic;

namespace Charon.Core
{
    public sealed class RuleSetRunner
    {
        private static readonly Lazy<RuleSetRunner> Lazy = new Lazy<RuleSetRunner>(() => new RuleSetRunner());

        public IList<IRuleSetHandler<IRuleRequest>> Rules { get; protected set; }

        public void AddRule(RequestAnalyserRuleSet<IRuleRequest> ruleSet)
        {
            Rules.Add((ruleSet));
        }

        public static RuleSetRunner Instance { get { return Lazy.Value; } }

        private RuleSetRunner()
        {
            Rules = new List<IRuleSetHandler<IRuleRequest>>();
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
