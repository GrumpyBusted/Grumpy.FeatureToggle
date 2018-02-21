using Grumpy.Entity.Interfaces;
using Grumpy.FeatureToggle.Core.Infrastructure;
using Grumpy.FeatureToggle.Entity;
using Microsoft.Extensions.Logging;

namespace Grumpy.FeatureToggle.Infrastructure.Repositories
{
    public class RepositoryContext : IRepositoryContext
    {
        private readonly Entities _entities;
        private bool _disposed;

        public RepositoryContext(ILogger logger, IEntityConnectionConfig entityConnectionConfig)
        {
            _entities = new Entities(logger, entityConnectionConfig);
        }

        public void Save()
        {
            _entities.SaveChanges();
        }

        public IFeatureToggleRepository FeatureToggle => new FeatureToggleRepository(_entities);

        public void Dispose()
        {
            Dispose(true);
        }

        private void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                    _entities.Dispose();
            }
        }
    }
}