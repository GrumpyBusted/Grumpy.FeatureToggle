namespace Grumpy.FeatureToggle.Api.Dto
{
    public class FeatureToggleRequest
    {
        public string ServiceName { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string Feature { get; set; }
    }
}