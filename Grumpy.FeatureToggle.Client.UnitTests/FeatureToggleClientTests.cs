using FluentAssertions;
using Grumpy.FeatureToggle.Api.Dto;
using Grumpy.FeatureToggle.Client.Interfaces;
using Grumpy.RipplesMQ.Client.Interfaces;
using Grumpy.RipplesMQ.Config;
using NSubstitute;
using Xunit;
using Microsoft.Extensions.Logging.Abstractions;

namespace Grumpy.FeatureToggle.Client.UnitTests
{
    public class FeatureToggleClientTests
    {
        private readonly IMessageBus _messageBus;
        private readonly IFeatureToggleClient _cut;

        public FeatureToggleClientTests()
        {
            _messageBus = Substitute.For<IMessageBus>();

            _cut = new FeatureToggleClient(NullLogger.Instance, _messageBus);
        }

        [Fact]
        public void ClientWithTrueFromFeatureToggleServiceShouldBeEnabled()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>()).Returns(new FeatureToggleResponse { Enabled = true });

            _cut.IsEnabled("MyFeature").Should().BeTrue();
        }

        [Fact]
        public void ClientWithTrueFromFeatureToggleServiceShouldBeDisabled()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>()).Returns(new FeatureToggleResponse { Enabled = true });

            _cut.IsDisabled("MyFeature").Should().BeFalse();
        }

        [Fact]
        public void ClientWithFalseFromFeatureToggleServiceShouldBeNotEnabled()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>()).Returns(new FeatureToggleResponse { Enabled = false });

            _cut.IsEnabled("MyFeature").Should().BeFalse();
        }

        [Fact]
        public void ClientWithSpecificNameShouldGetCorrectAnswer()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>()).Returns(new FeatureToggleResponse { Enabled = false });
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Is<FeatureToggleRequest>(e => e.Feature == "MyFeature")).Returns(new FeatureToggleResponse { Enabled = true });

            _cut.IsEnabled("MyFeature").Should().BeTrue();
        }

        [Fact]
        public void ClientWithHandlesShouldOnlyCallServiceOnce()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Is<FeatureToggleRequest>(e => e.Feature == "MyFeature")).Returns(new FeatureToggleResponse { Enabled = true });

            _cut.IsEnabled("MyFeature");
            _cut.IsEnabled("MyFeature");

            _messageBus.Received(1).Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>());
        }

        [Fact]
        public void ReceiveUpdateShouldChangeToggle()
        {
            _messageBus.Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Is<FeatureToggleRequest>(e => e.Feature == "MyFeature")).Returns(new FeatureToggleResponse { Enabled = false }, new FeatureToggleResponse { Enabled = true });

            _cut.IsEnabled("MyFeature").Should().BeFalse();

            _cut.FeatureToggleUpdated(new FeatureToggleChanged { Feature = "MyFeature" });

            _cut.IsEnabled("MyFeature").Should().BeTrue();
            _messageBus.Received(2).Request<FeatureToggleRequest, FeatureToggleResponse>(Arg.Any<RequestResponseConfig>(), Arg.Any<FeatureToggleRequest>());
        }
    }
}
