namespace WebMinder.Core
{
    public interface IRuleRunner
    {
        void VerifyRule(IRuleRequest request = null);
    }
}
