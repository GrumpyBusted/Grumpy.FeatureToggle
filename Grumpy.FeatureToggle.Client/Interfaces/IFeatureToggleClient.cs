using Grumpy.FeatureToggle.Api.Dto;

namespace Grumpy.FeatureToggle.Client.Interfaces
{
    public interface IFeatureToggleClient
    {
        bool IsEnabled(string feature);
        bool IsDisabled(string feature);
        void FeatureToggleUpdated(FeatureToggleChanged dto);
    }
}