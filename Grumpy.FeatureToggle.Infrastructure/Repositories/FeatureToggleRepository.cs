using System.Linq;
using Grumpy.FeatureToggle.Core.Infrastructure;
using Grumpy.FeatureToggle.Entity;

namespace Grumpy.FeatureToggle.Infrastructure.Repositories
{
    public class FeatureToggleRepository : IFeatureToggleRepository
    {
        private readonly Entities _entities;

        public FeatureToggleRepository(Entities entities)
        {
            _entities = entities;
        }

        public void Add(Entity.FeatureToggle featureToggle)
        {
            _entities.FeatureToggle.Add(featureToggle);
        }

        public IQueryable<Entity.FeatureToggle> GetAll()
        {
            return _entities.FeatureToggle;
        }
    }
}