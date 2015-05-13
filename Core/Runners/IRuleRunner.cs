using WebMinder.Core.Rules;

namespace WebMinder.Core.Runners
{
    public interface IRuleRunner
    {
        void VerifyRule(IRuleRequest request = null);
    }
}
