namespace WebMinder.Core.Handlers
{
    public interface IMaximumCountRuleSetHandler<T> : IRuleSetHandler<T> where T : IRuleRequest, new()
    {
        int? MaximumResultCount { get; set; }
    }
}