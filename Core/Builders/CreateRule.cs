using System;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Builders
{
    public class CreateRule<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler :  IRuleSetHandler<TRuleRequest>, new()
        where TRuleRequest : IRuleRequest, new()
    {
        private static TRuleSetHandler _rule;
        private static IRuleRequest _request;

        public TRuleSetHandler Rule
        {
            get { return _rule; }
        }

        public static CreateRule<TRuleSetHandler, TRuleRequest> On<T>(Action<T> setter = null) where T : IRuleRequest, new()
        {
            EnsureCreated(setter);
            return AppendRule();
        }

        public CreateRule<TRuleSetHandler, TRuleRequest> Using<T>(Action<T> setter = null) where T : IRuleRequest, new()
        {
            EnsureCreated(setter);
            return AppendRule();
        }

        private static void EnsureCreated<T>(Action<T> setter = null) where T : IRuleRequest, new()
        {
            if (_rule==null)_rule = Activator.CreateInstance<TRuleSetHandler>();
            if (_request == null) _request = Activator.CreateInstance<T>();
            if (setter != null) setter((T) _request);
        }

        internal void SetRule(TRuleSetHandler rule)
        {
            _rule = rule;
        }

        public CreateRule<TRuleSetHandler, TRuleRequest> With(Action<TRuleSetHandler> setter)
        {
            setter(_rule);
            return AppendRule();
        }

        public CreateRule<TRuleSetHandler, TRuleRequest> Build()
        {
            EnsureCreated<TRuleRequest>();
            return AppendRule();
        }

        private static CreateRule<TRuleSetHandler, TRuleRequest> AppendRule()
        {
            var result = new CreateRule<TRuleSetHandler, TRuleRequest>();
            result.SetRule(_rule);
            return result;
        }
    }
}