using System;

namespace Grumpy.FeatureToggle.Core.Infrastructure
{
    public interface IRepositoryContext : IDisposable
    {
        void Save();

        IFeatureToggleRepository FeatureToggle { get; }
    }
}