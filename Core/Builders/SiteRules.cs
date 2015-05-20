using System;
using System.Collections.Generic;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public class SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
        where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
        where TRuleRequest : IRuleRequest, new()
    {

        private static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> _self;
        public List<CreateRule<IRuleSetHandler<RuleRequest>, RuleRequest>> RuleSets { get; set; }

        public static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> Create()
        {
            _self = new SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
            {
                RuleSets = new List<CreateRule<IRuleSetHandler<RuleRequest>, RuleRequest>>()
            };
            return _self;
        }

        public SiteRules<TCreate, TRuleSetHandler, TRuleRequest> WithRule<TRuleSetHandler2, TRuleRequest2>(Func<CreateRule<TRuleSetHandler2, TRuleRequest2>> setter)
        where TRuleSetHandler2 : IRuleSetHandler<TRuleRequest2>
        where TRuleRequest2 : RuleRequest, new()
        {
            var fnc = setter() as CreateRule<IRuleSetHandler<RuleRequest>, RuleRequest>;
            _self.RuleSets.Add(fnc);
            return this;
        }
    }


    public static class SiteRulesExtensions
    {
        public static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> And<TRuleMinder, TCreate, TRuleSetHandler, TRuleRequest>(this SiteRules<TCreate, TRuleSetHandler, TRuleRequest> rules)
            where TRuleMinder : RuleMinder, new()
            where TCreate : CreateRule<TRuleSetHandler, TRuleRequest>
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

        //public static CreateRule<IRuleSetHandler<RuleRequest>, RuleRequest> AsMindedRule<TRuleSetHandler, TRuleRequest>(this CreateRule<TRuleSetHandler, TRuleRequest> uncastRule)
        //    where TRuleRequest : RuleRequest, IRuleRequest, new()
        //    where TRuleSetHandler : RuleSetHandlerBase<RuleRequest>,  IRuleSetHandler<RuleRequest>, IRuleSetHandler<TRuleRequest>
        //{

        //    return (CreateRule<IRuleSetHandler<RuleRequest>, RuleRequest>)uncastRule;
        //}
    }
}


  //where TCreate1 : CreateRule<TRuleSetHandler1, TRuleRequest1>
  //          where TRuleSetHandler1 : IRuleSetHandler<TRuleRequest1>
  //          where TRuleRequest1 : IRuleRequest, new()