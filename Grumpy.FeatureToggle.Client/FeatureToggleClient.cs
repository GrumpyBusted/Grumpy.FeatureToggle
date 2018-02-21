using System.Collections.Concurrent;
using System.Collections.Generic;
using Grumpy.Common;
using Grumpy.FeatureToggle.Api;
using Grumpy.FeatureToggle.Api.Dto;
using Grumpy.FeatureToggle.Client.Interfaces;
using Grumpy.Logging;
using Grumpy.RipplesMQ.Client.Interfaces;
using Microsoft.Extensions.Logging;

namespace Grumpy.FeatureToggle.Client
{
    public class FeatureToggleClient : IFeatureToggleClient
    {
        private readonly ILogger _logger;
        private readonly IMessageBus _messageBus;
        private readonly string _serviceName;
        private readonly string _userName;
        private readonly string _machineName;
        private readonly IDictionary<string, bool> _featureToggles;

        public FeatureToggleClient(ILogger logger, IMessageBus messageBus, string serviceName = null, string userName = null, string machineName = null)
        {
            _logger = logger;
            _messageBus = messageBus;

            var processInformation = new ProcessInformation();

            _serviceName = serviceName ?? processInformation.ProcessName;
            _userName = userName ?? processInformation.UserName;
            _machineName = machineName ?? processInformation.MachineName;

            _featureToggles = new ConcurrentDictionary<string, bool>();

            _messageBus.SubscribeHandler<FeatureToggleChanged>(FeatureToggleConfig.FeatureToggleChanged, FeatureToggleUpdated);
        }

        public bool IsDisabled(string feature)
        {
            return !IsEnabled(feature);
        }

        public void FeatureToggleUpdated(FeatureToggleChanged dto)
        {
            if (_featureToggles.ContainsKey(dto.Feature))
                UpdateCache(dto.Feature, Get(dto.Feature));
        }

        public bool IsEnabled(string feature)
        {
            if (!_featureToggles.ContainsKey(feature))
            {
                var enabled = Get(feature);

                UpdateCache(feature, enabled);

                return enabled;
            }

            return _featureToggles[feature];
        }

        private bool Get(string feature)
        {
            var request = new FeatureToggleRequest
            {
                ServiceName = _serviceName,
                UserName = _userName,
                MachineName = _machineName,
                Feature = feature
            };

            var response = _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(FeatureToggleConfig.RequestFeatureToggle, request);

            _logger.Information("Feature Toggle {Feature} {Enabled} {@Request}", feature, response.Enabled, request);

            return response.Enabled;
        }

        private void UpdateCache(string feature, bool enabled)
        {
            _featureToggles[feature] = enabled;
        }
    }
}
