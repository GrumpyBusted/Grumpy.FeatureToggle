namespace Grumpy.FeatureToggle.Core.Infrastructure
{
    public interface IRepositoryContextFactory
    {
        IRepositoryContext Get();
    }
}