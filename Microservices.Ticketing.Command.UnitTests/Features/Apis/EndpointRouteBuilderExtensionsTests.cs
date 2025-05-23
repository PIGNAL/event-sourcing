using Microsoft.AspNetCore.Routing;
using Moq;
using Ticketing.Command.Features.Apis;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Features.Apis
{
    public class EndpointRouteBuilderExtensionsTests
    {
        [Fact]
        public void MapMinimalApisEndpoints_CallsAddEndpointOnAllMinimalApis()
        {
            // Arrange
            var minimalApiMock1 = new Mock<IMinimalApi>();
            var minimalApiMock2 = new Mock<IMinimalApi>();
            var minimalApis = new List<IMinimalApi> { minimalApiMock1.Object, minimalApiMock2.Object };

            var serviceProviderMock = new Mock<IServiceProvider>();
            serviceProviderMock
                .Setup(sp => sp.GetService(typeof(IEnumerable<IMinimalApi>)))
                .Returns(minimalApis);

            var endpointRouteBuilderMock = new Mock<IEndpointRouteBuilder>();
            endpointRouteBuilderMock
                .SetupGet(x => x.ServiceProvider)
                .Returns(serviceProviderMock.Object);

            // Act
            var result = endpointRouteBuilderMock.Object.MapMinimalApisEndpoints();

            // Assert
            minimalApiMock1.Verify(m => m.AddEndpoint(endpointRouteBuilderMock.Object), Times.Once);
            minimalApiMock2.Verify(m => m.AddEndpoint(endpointRouteBuilderMock.Object), Times.Once);
            Assert.Equal(endpointRouteBuilderMock.Object, result);
        }
    }
}
