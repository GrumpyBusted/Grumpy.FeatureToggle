using System.Linq;

namespace Grumpy.FeatureToggle.Core.Infrastructure
{
    public interface IFeatureToggleRepository
    {
        void Add(Entity.FeatureToggle featureToggle);
        IQueryable<Entity.FeatureToggle> GetAll();
    }
}
