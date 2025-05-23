using Common.Core.Producers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ticketing.Command.Application;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Application
{
    public class ApplicationServiceRegistrationTests
    {
        [Fact]
        public void ApplicationServiceRegistration_Configuration_IsValid()
        {
            // Arrange
            var services = new ServiceCollection();
            var configuration = new ConfigurationBuilder().Build();
            // Act
            services.AddApplicationServices(configuration);
            var serviceProvider = services.BuildServiceProvider();
            // Assert

            Assert.NotNull(serviceProvider.GetService<MediatR.IMediator>());
            Assert.NotNull(serviceProvider.GetServices<FluentValidation.IValidator<object>>());
            Assert.NotNull(serviceProvider.GetService<AutoMapper.IMapper>());
            Assert.NotNull(serviceProvider.GetService<IOptions<MongoSettings>>());
            Assert.NotNull(serviceProvider.GetService<IOptions<KafkaSettings>>());

        }
    }
}
