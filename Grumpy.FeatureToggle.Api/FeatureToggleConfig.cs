using Grumpy.RipplesMQ.Config;

namespace Grumpy.FeatureToggle.Api
{
    public static class FeatureToggleConfig
    {
        public static readonly PublishSubscribeConfig UpdateFeatureToggle = new PublishSubscribeConfig
        {
            Topic = "UpdateFeatureToggle",
            Persistent = true
        };

        public static readonly PublishSubscribeConfig FeatureToggleChanged = new PublishSubscribeConfig
        {
            Topic = "FeatureToggleChanged",
            Persistent = false
        };

        public static readonly RequestResponseConfig RequestFeatureToggle = new RequestResponseConfig
        {
            Name = "RequestFeatureToggle",
            MillisecondsTimeout = 5000
        };
    }
}
