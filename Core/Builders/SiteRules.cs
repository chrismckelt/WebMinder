using System;
using System.Collections.Generic;
using System.Linq;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public class SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
        where TCreate : Create<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
        where TRuleRequest : IRuleRequest, new()
    {

        private static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> _self;
        public List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>> RuleSets { get; set; }

        public static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> Create()
        {
            _self = new SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
            {
                RuleSets = new List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>>()
            };
            return _self;
        }

        public SiteRules<TCreate, TRuleSetHandler, TRuleRequest> WithRule<TRuleSetHandler2, TRuleRequest2>(Func<Create<TRuleSetHandler2, TRuleRequest2>> setter)
        where TRuleSetHandler2 : IRuleSetHandler<TRuleRequest2>
        where TRuleRequest2 : RuleRequest, new()
        {
            var fnc = setter() as Create<IRuleSetHandler<RuleRequest>, RuleRequest>;
            _self.RuleSets.Add(fnc);
            return this;
        }
    }


    public class RuleMinder 
    {
        public IList<IRuleSetHandler<RuleRequest>> MindedRules { get; set; }

        public static RuleMinder Create()
        {
            var minder = new RuleMinder();
            minder.MindedRules = new List<IRuleSetHandler<RuleRequest>>();
            return minder;
        }

        public void Add(IEnumerable<IRuleSetHandler<RuleRequest>> minded) 
        {
            minded.ToList().ForEach(x=>MindedRules.Add(x));
        }

        public RuleMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<Create<TRuleSetHandler2, TRuleRequest2>> setter)
            where TRuleSetHandler2 : IRuleSetHandler<TRuleRequest2>
            where TRuleRequest2 : RuleRequest, new()
        {
            var fnc = setter() as Create<IRuleSetHandler<RuleRequest>, RuleRequest>;
            MindedRules.Add(fnc.Rule);
            return this;
        }
    }

    public static class RuleMinderExtensions
    {
        public static RuleMinder And<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder ruleMinder)
            where TCreate : Create<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
            where TRuleRequest : IRuleRequest, new()
        {
            var builder = SiteRules<TCreate, TRuleSetHandler, TRuleRequest>.Create();
            var rules = builder.RuleSets.Select(x => x.Rule);
            ruleMinder.Add(rules);
            return ruleMinder;
        }


        public static RuleMinder And<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder ruleMinder, Func<Create<TRuleSetHandler, TRuleRequest>> setter)
            where TCreate : Create<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
            where TRuleRequest : IRuleRequest, new()
        {
            var rule = setter().Rule as IRuleSetHandler<RuleRequest>;
            var list = new List<IRuleSetHandler<RuleRequest>> {rule};
            ruleMinder.Add(list);
            return ruleMinder;
        }

    }

    public static class SiteRulesExtensions
    {
        public static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> And<TRuleMinder, TCreate, TRuleSetHandler, TRuleRequest>(this SiteRules<TCreate, TRuleSetHandler, TRuleRequest> rules)
            where TRuleMinder : RuleMinder, new()
            where TCreate : Create<TRuleSetHandler, TRuleRequest>
            where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
            where TRuleRequest : IRuleRequest, new()
        {
            var rule = SiteRules<TCreate, TRuleSetHandler, TRuleRequest>.Create();
            foreach (var rr in rules.RuleSets)
            {
                rule.RuleSets.Add(rr);
            }
            return rule;
        }

        //public static Create<IRuleSetHandler<RuleRequest>, RuleRequest> AsMindedRule<TRuleSetHandler, TRuleRequest>(this Create<TRuleSetHandler, TRuleRequest> uncastRule)
        //    where TRuleRequest : RuleRequest, IRuleRequest, new()
        //    where TRuleSetHandler : RuleSetHandlerBase<RuleRequest>,  IRuleSetHandler<RuleRequest>, IRuleSetHandler<TRuleRequest>
        //{

        //    return (Create<IRuleSetHandler<RuleRequest>, RuleRequest>)uncastRule;
        //}
    }
}


  //where TCreate1 : Create<TRuleSetHandler1, TRuleRequest1>
  //          where TRuleSetHandler1 : IRuleSetHandler<TRuleRequest1>
  //          where TRuleRequest1 : IRuleRequest, new()