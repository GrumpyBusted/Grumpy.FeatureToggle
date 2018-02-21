using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Grumpy.FeatureToggle.Api;
using Grumpy.FeatureToggle.Api.Dto;
using Grumpy.FeatureToggle.Core.Infrastructure;
using Grumpy.FeatureToggle.Core.Interfaces;
using Grumpy.RipplesMQ.Client.Interfaces;
using Grumpy.RipplesMQ.Config;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Xunit;

namespace Grumpy.FeatureToggle.Core.UnitTests
{
    public class FeatureToggleTests
    {
        private readonly IFeatureToggle _cut;
        private readonly IMessageBus _messageBus;
        private readonly IFeatureToggleRepository _featureToggleRepository;
        private readonly IRepositoryContext _repositoryContext;

        public FeatureToggleTests()
        {
            _featureToggleRepository = Substitute.For<IFeatureToggleRepository>();

            var logger = Substitute.For<ILogger>();
            _messageBus = Substitute.For<IMessageBus>();
            var repositoryContextFactory = Substitute.For<IRepositoryContextFactory>();
            _repositoryContext = Substitute.For<IRepositoryContext>();

            repositoryContextFactory.Get().Returns(_repositoryContext);
            _repositoryContext.FeatureToggle.Returns(_featureToggleRepository);

            _cut = new FeatureToggle(logger, _messageBus, repositoryContextFactory);
        }

        [Fact]
        public void NonDefinedFeatureToggleShouldBeDisabled()
        {
            _cut.IsEnabled(new FeatureToggleRequest()).Enabled.Should().BeFalse();
        }

        [Fact]
        public void DefinedFeatureToggleShouldBeEnabled()
        {
            var featureToggles = new List<Entity.FeatureToggle>
            {
                new Entity.FeatureToggle
                {
                    Feature = "MyFeature",
                    Enabled = true
                }
            };

            _featureToggleRepository.GetAll().Returns(featureToggles.AsQueryable());

            _cut.IsEnabled(new FeatureToggleRequest { Feature = "MyFeature" }).Enabled.Should().BeTrue();
        }

        [Fact]
        public void UseDefaultWhenKeysNotFoundShouldWork()
        {
            var featureToggles = new List<Entity.FeatureToggle>
            {
                new Entity.FeatureToggle
                {
                    MachineName = "*",
                    UserName = "*",
                    ServiceName = "*",
                    Feature = "MyFeature",
                    Enabled = true
                }
            };

            _featureToggleRepository.GetAll().Returns(featureToggles.AsQueryable());

            _cut.IsEnabled(new FeatureToggleRequest { MachineName = "X", UserName = "Y", ServiceName = "Z", Feature = "MyFeature" }).Enabled.Should().BeTrue();
        }

        [Fact]
        public void UseFeatureWithLowersPriority()
        {
            var featureToggles = new List<Entity.FeatureToggle>
            {
                new Entity.FeatureToggle
                {
                    MachineName = "*",
                    UserName = "*",
                    ServiceName = "*",
                    Feature = "MyFeature",
                    Enabled = true,
                    Priority = 2
                },
                new Entity.FeatureToggle
                {
                    MachineName = "*",
                    UserName = "*",
                    ServiceName = "*",
                    Feature = "MyFeature",
                    Enabled = false,
                    Priority = 1
                }
            };

            _featureToggleRepository.GetAll().Returns(featureToggles.AsQueryable());

            _cut.IsEnabled(new FeatureToggleRequest { MachineName = "X", UserName = "Y", ServiceName = "Z", Feature = "MyFeature" }).Enabled.Should().BeFalse();
        }

        [Fact]
        public void UpdateNewFeatureToggleShouldAddToRepository()
        {
            _cut.Update(new Api.Dto.FeatureToggle());

            _featureToggleRepository.Received(1).Add(Arg.Any<Entity.FeatureToggle>());
        }

        [Fact]
        public void UpdateExistingFeatureToggleShouldSaveRepository()
        {
            var featureToggles = new List<Entity.FeatureToggle>
            {
                new Entity.FeatureToggle
                {
                    Feature = "MyFeature",
                    Enabled = true
                }
            };

            _featureToggleRepository.GetAll().Returns(featureToggles.AsQueryable());

            _cut.Update(new Api.Dto.FeatureToggle { Feature = "MyFeature", Enabled = false });

            _featureToggleRepository.Received(0).Add(Arg.Any<Entity.FeatureToggle>());
            _repositoryContext.Received(1).Save();
        }

        [Fact]
        public void UpdateShouldPublishEvent()
        {
            _cut.Update(new Api.Dto.FeatureToggle());

            _messageBus.Received(1).Publish(Arg.Is<PublishSubscribeConfig>(c => c.Topic == FeatureToggleConfig.FeatureToggleChanged.Topic), Arg.Any<FeatureToggleChanged>());
        }
    }
}
