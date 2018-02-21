using Grumpy.Entity.Interfaces;
using Grumpy.FeatureToggle.Core.Infrastructure;
using Microsoft.Extensions.Logging;

namespace Grumpy.FeatureToggle.Infrastructure.Repositories
{
    public class RepositoryContextFactory : IRepositoryContextFactory
    {
        private readonly ILogger _logger;
        private readonly IEntityConnectionConfig _entityConnectionConfig;

        public RepositoryContextFactory(ILogger logger, IEntityConnectionConfig entityConnectionConfig)
        {
            _logger = logger;
            _entityConnectionConfig = entityConnectionConfig;
        }

        public IRepositoryContext Get()
        {
            return new RepositoryContext(_logger, _entityConnectionConfig);
        }
    }
}