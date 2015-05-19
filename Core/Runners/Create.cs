using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Runners
{

    public class SiteRules<TCreate, TRuleSetHandler, TRuleRequest>
        where TCreate : Create<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
        where TRuleRequest : IRuleRequest, new()
    {
        private static List<Create<TRuleSetHandler, TRuleRequest>> Rules { get; set; }


        public SiteRules<TCreate, TRuleSetHandler, TRuleRequest> AddRule<TCreate1, TRuleSetHandler1, TRuleRequest1>(Func<Create<TRuleSetHandler1, TRuleRequest1>> setter)
            where TCreate1 : Create<TRuleSetHandler1, TRuleRequest1>
            where TRuleSetHandler1 : IRuleSetHandler<TRuleRequest1>
            where TRuleRequest1 : IRuleRequest, new()
        {
            //Rules.Add(setter();
            return this;
        }
 
    }

    public class Create<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
        where TRuleRequest : IRuleRequest, new()
    {
        private static TRuleSetHandler _rule;
        private static IRuleRequest _request;

        public TRuleSetHandler Rule
        {
            get { return _rule; }
        }

        public static Create<TRuleSetHandler, TRuleRequest> On<T>(Action<T> setter = null) where T : IRuleRequest, new()
        {
            _rule = Activator.CreateInstance<TRuleSetHandler>();
            _request = Activator.CreateInstance<T>();
            if (setter != null) setter((T)_request);
            return AppendRule();
        }

        internal void SetRule(TRuleSetHandler rule)
        {
            _rule = rule;
        }

        public Create<TRuleSetHandler, TRuleRequest> With(Action<TRuleSetHandler> setter)
        {
            setter(_rule);
            return AppendRule();
        }

        public Create<TRuleSetHandler, TRuleRequest> Build()
        {
            return AppendRule();
        }

        public static Create<TRuleSetHandler, TRuleRequest> AppendRule()
        {
            var result = new Create<TRuleSetHandler, TRuleRequest>();
            result.SetRule(_rule);
            return result;
        }
    }
}