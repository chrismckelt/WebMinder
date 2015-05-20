using System;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
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