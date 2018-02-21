using Grumpy.FeatureToggle.Api.Dto;

namespace Grumpy.FeatureToggle.Core.Interfaces
{
    public interface IFeatureToggle
    {
        FeatureToggleResponse IsEnabled(FeatureToggleRequest request);
        void Update(Api.Dto.FeatureToggle dto);
    }
}