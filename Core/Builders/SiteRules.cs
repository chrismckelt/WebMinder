using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        public List<SiteRules<TCreate, TRuleSetHandler, TRuleRequest>> SiteRulesCollection { get; set; }
        public List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>> RuleSets { get; set; }

        public static SiteRules<TCreate, TRuleSetHandler, TRuleRequest> Create()
        {
            _self = new SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
            {
                SiteRulesCollection = new List<SiteRules<TCreate, TRuleSetHandler, TRuleRequest>>(),
                RuleSets = new List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>>()
            };
            return _self;
        }

        public RuleMinder WithRule<TRuleSetHandler2, TRuleRequest2>(Func<Create<TRuleSetHandler2, TRuleRequest2>> setter)
        where TRuleSetHandler2 : IRuleSetHandler<TRuleRequest2>
        where TRuleRequest2 : RuleRequest, new()
        {
            var fnc = setter() as Create<IRuleSetHandler<RuleRequest>, RuleRequest>;
            _self.RuleSets.Add(fnc);
            return new RuleMinder()
            {
                RuleSets = _self.RuleSets,
              //  SiteRulesCollection = _self.SiteRulesCollection
            };
        }
    }


    public class RuleMinder : SiteRules<Create<RuleSetHandlerBase<RuleRequest>,RuleRequest>, RuleSetHandlerBase<RuleRequest>, RuleRequest>  
    {
        //public void Swallow<T1, T2, T3>(SiteRules<T1, T2, T3> siteRules)
        //    where T1 : Create<T2, T3>, new()
        //    where T2 : RuleSetHandlerBase<T3>, IRuleSetHandler<T3>, new()
        //    where T3 : RuleRequest, IRuleRequest, new()
        //{
            
        //}
    }

    public static class SiteRulesExtensions
    {
        public static RuleMinder ToMinded<TCreate, TRuleSetHandler, TRuleRequest>(
            SiteRules<TCreate, TRuleSetHandler, TRuleRequest> arg)
            where TCreate : Create<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : RuleSetHandlerBase<TRuleRequest>, IRuleSetHandler<TRuleRequest>, new()
            where TRuleRequest : RuleRequest, IRuleRequest, new()
        {
            var minder = new RuleMinder()
            {
                RuleSets =  new List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>>(),
                SiteRulesCollection = new List<SiteRules<Create<RuleSetHandlerBase<RuleRequest>, RuleRequest>, RuleSetHandlerBase<RuleRequest>, RuleRequest>>()
            };

            foreach (var rs in arg.RuleSets)
            {
                minder.RuleSets.Add(rs);
            }

            return minder;
        }

        public static RuleMinder And<TCreate, TRuleSetHandler, TRuleRequest>(this RuleMinder minder)
            where TCreate : Create<TRuleSetHandler, TRuleRequest>, new()
            where TRuleSetHandler : RuleSetHandlerBase<TRuleRequest>, IRuleSetHandler<TRuleRequest>,  new()
            where TRuleRequest : RuleRequest,IRuleRequest, new()

        {
            minder.SiteRulesCollection = new List<SiteRules<Create<RuleSetHandlerBase<RuleRequest>, RuleRequest>, RuleSetHandlerBase<RuleRequest>,RuleRequest>>();
            minder.RuleSets = new List<Create<IRuleSetHandler<RuleRequest>, RuleRequest>>();
            return minder;
        }

        public static RuleMinder And<TRuleMinder, TCreate, TRuleSetHandler, TRuleRequest>(this SiteRules<TCreate, TRuleSetHandler, TRuleRequest> rules)
            where TRuleMinder : RuleMinder, new()
            where TCreate : Create<TRuleSetHandler, TRuleRequest>
            where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
            where TRuleRequest : IRuleRequest, new()
        {
            var rule = SiteRules<TCreate, TRuleSetHandler, TRuleRequest>.Create();
            rules.SiteRulesCollection.Add(rule);
            var minder = new RuleMinder();
            return (TRuleMinder)minder;
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