using System;
using System.Configuration;
using System.Threading;
using Grumpy.Entity;
using Grumpy.FeatureToggle.Infrastructure.Repositories;
using Grumpy.RipplesMQ.Client;
using Grumpy.RipplesMQ.Client.Interfaces;
using Grumpy.ServiceBase;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Console;

namespace Grumpy.FeatureToggle.Service
{
    public class FeatureToggleService : TopshelfServiceBase, IDisposable
    {
        private IMessageBus _messageBus;
        private Core.FeatureToggle _featureToggle;
        private bool _disposed;
        private LogLevel _logLevel = LogLevel.Warning;

        protected override void Process(CancellationToken cancellationToken)
        {
            Logger = new ConsoleLogger(ServiceName, (message, level) => level >= _logLevel, false);

            var appSettings = ConfigurationManager.AppSettings;

            Enum.TryParse(appSettings["LogLevel"], true, out _logLevel);

            var entityConnectionConfig = new EntityConnectionConfig(new DatabaseConnectionConfig(appSettings["DatabaseServer"], appSettings["DatabaseName"]));
            var repositoryContextFactory = new RepositoryContextFactory(Logger, entityConnectionConfig);
            var messageBusBuilder = new MessageBusBuilder();

            messageBusBuilder.WithLogger(Logger);
            messageBusBuilder.WithServiceName(ServiceName);

            _messageBus = messageBusBuilder.Build();

            _featureToggle = new Core.FeatureToggle(Logger, _messageBus, repositoryContextFactory);

            _messageBus.Start(cancellationToken);
        }

        public new void Dispose()
        {
            Dispose(true);
        }

        protected override void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                _disposed = true;

                if (disposing)
                    _messageBus?.Dispose();

                base.Dispose(disposing);
            }
        }
    }
}