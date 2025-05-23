using Microsoft.Extensions.DependencyInjection;
using Ticketing.Command.Features.Apis;
using Ticketing.Command.Features.Tickets;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Features.Apis
{
    public class ServiceCollectionExtensionsTests
    {
        [Fact]
        public void RegisterMinimalApis_RegistersAllMinimalApis()
        {
            // Arrange
            var services = new ServiceCollection();

            // Act
            services.RegisterMinimalApis();
            var provider = services.BuildServiceProvider();

            // Assert
            var apis = provider.GetServices<IMinimalApi>();
            Assert.Contains(apis, api => api is TicketCreate);
        }

    }
}
