namespace Grumpy.FeatureToggle.Api.Dto
{
    public class FeatureToggle
    {
        public string ServiceName { get; set; }
        public string UserName { get; set; }
        public string MachineName { get; set; }
        public string Feature { get; set; }
        public int Priority { get; set; }
        public bool Enabled { get; set; }
    }
}