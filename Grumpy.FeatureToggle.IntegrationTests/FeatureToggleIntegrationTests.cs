using System.Threading;
using FluentAssertions;
using Grumpy.Entity;
using Grumpy.FeatureToggle.Api.Dto;
using Grumpy.FeatureToggle.Core.Interfaces;
using Grumpy.FeatureToggle.Infrastructure.Repositories;
using Grumpy.RipplesMQ.Client;
using Microsoft.Extensions.Logging.Abstractions;
using Xunit;

namespace Grumpy.FeatureToggle.IntegrationTests
{
    public class FeatureToggleIntegrationTests
    {
        private readonly IFeatureToggle _cut;

        public FeatureToggleIntegrationTests()
        {
            var logger = NullLogger.Instance;

            var entityConnectionConfig = new EntityConnectionConfig(new DatabaseConnectionConfig(@"(localdb)\MSSQLLocalDB", "Grumpy.FeatureToggle.Database_Model"));
            var repositoryContextFactory = new RepositoryContextFactory(logger, entityConnectionConfig);
            var messageBusBuilder = new MessageBusBuilder();

            var messageBus = messageBusBuilder.Build();

            _cut = new Core.FeatureToggle(logger, messageBus, repositoryContextFactory);

            messageBus.Start(new CancellationToken());
        }

        [Fact]
        public void CanUpdateDatabase()
        {
            _cut.Update(new Api.Dto.FeatureToggle { MachineName = "X", ServiceName = "FeatureToggleIntegrationTest", UserName = "Z", Feature = "MyFeature", Enabled = true, Priority = 1 });

            _cut.IsEnabled(new FeatureToggleRequest { MachineName = "X", ServiceName = "FeatureToggleIntegrationTest", UserName = "Z", Feature = "MyFeature" }).Enabled.Should().BeTrue();
        }
    }
}
