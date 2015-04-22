namespace WebMinder.Core
{
    public interface IRuleRunner
    {
        void Run(IRuleRequest request,  bool addRequestToItemsCollection = true);
    }
}
