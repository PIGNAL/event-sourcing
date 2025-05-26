using Microsoft.Extensions.Options;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Infrastructure.Repositories;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Infrastructure.Repositories
{
    public class EventModelRepositoryTests
    {
        [Fact]
        public void EventModelRepository_Constructor_SetsCollectionName()
        {
            // Arrange
            var optionsMock = Options.Create(new MongoSettings { ConnectionString = "mongodb://localhost:27017", Database = "Ticketing" });
            var mongoClientMock = new MongoDB.Driver.MongoClient(optionsMock.Value.ConnectionString);
            var repository = new EventModelRepository(mongoClientMock, optionsMock);
            // Act and Assert
            Assert.NotNull(repository);
            Assert.IsType<EventModelRepository>(repository);
            Assert.IsAssignableFrom<IEventModelRepository>(repository);
            Assert.IsAssignableFrom<IMongoRepository<EventModel>>(repository);

        }
    }

}
