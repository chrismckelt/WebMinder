using System;
using WebMinder.Core.Handlers;
using WebMinder.Core.Rules;

namespace WebMinder.Core.Runners
{
    public class CreateRule<TRuleSetHandler, TRuleRequest>
        where TRuleSetHandler : IRuleSetHandler<TRuleRequest>
        where TRuleRequest : IRuleRequest, new()
    {
        private static TRuleSetHandler _rule;

        public TRuleSetHandler Rule
        {
            get { return _rule; }
        }

        public static CreateRule<TRuleSetHandler, TRuleRequest> For() // where TRuleRequest : IRuleRequest, new()
        {
            _rule = Activator.CreateInstance<TRuleSetHandler>();
            return AppendRule();
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
            RuleSetRunner.Instance.AddRule<TRuleSetHandler>(_rule);
            return AppendRule();
        }

        public static CreateRule<TRuleSetHandler, TRuleRequest> AppendRule()
        {
            var result = new CreateRule<TRuleSetHandler, TRuleRequest>();
            result.SetRule(_rule);
            return result;
        }
    }
}