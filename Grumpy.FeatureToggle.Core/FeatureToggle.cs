using System.Linq;
using Grumpy.FeatureToggle.Api;
using Grumpy.FeatureToggle.Api.Dto;
using Grumpy.FeatureToggle.Core.Infrastructure;
using Grumpy.FeatureToggle.Core.Interfaces;
using Grumpy.Logging;
using Grumpy.RipplesMQ.Client.Interfaces;
using Microsoft.Extensions.Logging;

namespace Grumpy.FeatureToggle.Core
{
    public class FeatureToggle : IFeatureToggle
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly IRepositoryContextFactory _repositoryContextFactory;

        public FeatureToggle(ILogger logger, IMessageBus messageBus, IRepositoryContextFactory repositoryContextFactory)
        {
            _logger = logger;
            _messageBus = messageBus;
            _repositoryContextFactory = repositoryContextFactory;

            messageBus.SubscribeHandler<Api.Dto.FeatureToggle>(FeatureToggleConfig.UpdateFeatureToggle, Update);
            messageBus.RequestHandler<FeatureToggleRequest, FeatureToggleResponse>(FeatureToggleConfig.RequestFeatureToggle, IsEnabled);
        }

        public FeatureToggleResponse IsEnabled(FeatureToggleRequest request)
        {
            var response = new FeatureToggleResponse
            {
                Enabled = IsEnabled(request.ServiceName, request.UserName, request.MachineName, request.Feature)
            };
            
            _logger.Debug("FeatureToggle retrieved {@Request} {@Response}", request, response);

            return response;
        }

        public void Update(Api.Dto.FeatureToggle dto)
        {
            Update(dto.ServiceName, dto.UserName, dto.MachineName, dto.Feature, dto.Priority, dto.Enabled);
        }

        private bool IsEnabled(string serviceName, string userName, string machineName, string feature)
        {
            using (var repositories = _repositoryContextFactory.Get())
            {
                var featureToggle = repositories.FeatureToggle.GetAll().Where(e => (e.ServiceName == serviceName || e.ServiceName == "*") && (e.UserName == userName || e.UserName == "*") && (e.MachineName == machineName || e.MachineName == "*") && e.Feature == feature).OrderBy(f => f.Priority).FirstOrDefault();

                return featureToggle?.Enabled ?? false;
            }
        }

        private void Update(string serviceName, string userName, string machineName, string feature, int priority, bool enabled)
        {
            using (var repositories = _repositoryContextFactory.Get())
            {
                var featureToggleRepository = repositories.FeatureToggle;

                var featureToggle = featureToggleRepository.GetAll().FirstOrDefault(e => e.ServiceName == serviceName && e.UserName == userName && e.MachineName == machineName && e.Feature == feature);

                if (featureToggle == null)
                {
                    featureToggle = new Entity.FeatureToggle
                    {
                        ServiceName = serviceName,
                        UserName = userName,
                        MachineName = machineName,
                        Feature = feature,
                        Enabled = enabled,
                        Priority = priority
                    };

                    _logger.Information("FeatureToggle added {@FeatureToggle}", featureToggle);

                    featureToggleRepository.Add(featureToggle);
                }
                else
                {
                    featureToggle.Enabled = enabled;
                    featureToggle.Priority = priority;

                    _logger.Information("FeatureToggle updated {@FeatureToggle}", featureToggle);
                }

                repositories.Save();
            }

            _messageBus.Publish(FeatureToggleConfig.FeatureToggleChanged, new FeatureToggleChanged { Feature = feature });
        }
    }
}
