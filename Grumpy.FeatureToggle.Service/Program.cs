using Grumpy.ServiceBase;

namespace Grumpy.FeatureToggle.Service
{
    public static class Program
    {
        public static void Main()
        {
            TopshelfUtility.Run<FeatureToggleService>();
        }
    }
}
