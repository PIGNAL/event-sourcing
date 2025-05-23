using Common.Core.Events;
using Common.Core.Producers;
using Microsoft.Extensions.Options;
using Moq;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.EventModels;
using Ticketing.Command.Infrastructure.Persistence;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Infrastructure.Persistence
{
    public class EventStoreTests
    {
        private readonly Mock<IEventModelRepository> _eventModelRepositoryMock;
        private readonly IOptions<KafkaSettings> _kafkaSettingsMock;
        private readonly Mock<IEventProducer> _eventProducerMock;

        public EventStoreTests()
        {
            _eventModelRepositoryMock = new Mock<IEventModelRepository>();
            _eventProducerMock = new Mock<IEventProducer>();
            _kafkaSettingsMock = Options.Create(new KafkaSettings
            {
                Hostname = "localhost",
                Topic = "test-topic",
                Port = "9092"
            });
        }

        [Fact]
        public async Task GetEventsAsync_Should()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            var aggregateId = Guid.NewGuid().ToString();
            var expectedEvents = new List<EventModel>
            {
                new EventModel { AggregateIdentifier = aggregateId, EventType = "TestEvent" }
            };
            _eventModelRepositoryMock
                .Setup(repo => repo.FilterByAsync(x => x.AggregateIdentifier == aggregateId, CancellationToken.None))
                .ReturnsAsync(expectedEvents);
            // Act
            var result = await eventStore.GetEventsAsync(aggregateId, CancellationToken.None);
            // Assert
            Assert.NotNull(result);
            Assert.Equal(expectedEvents.Count, result.Count());
        }

        [Fact]
        public async Task GetEventsAsync_Should_ReturnEmptyList_WhenNoEventsFound()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            var aggregateId = Guid.NewGuid().ToString();
            _eventModelRepositoryMock
                .Setup(repo => repo.FilterByAsync(x => x.AggregateIdentifier == aggregateId, CancellationToken.None))
                .ReturnsAsync(new List<EventModel>());
            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() => eventStore.GetEventsAsync(aggregateId, CancellationToken.None));
        }

        [Fact]
        public async Task SaveEventAsync_Should_SaveEventAndProduceMessage()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            var aggregateId = Guid.NewGuid().ToString();
            var expectedEvents = new List<EventModel>
            {
                new EventModel { AggregateIdentifier = aggregateId, EventType = "TestEvent", Version = 1 }
            };
            var events = new List<BaseEvent>
            {
                new TicketCreatedEvent
                    { Version = 1, UserName = "test@email.com", TypeError = 0, DetailError = "detail" },
            };
            _eventModelRepositoryMock
                .Setup(repo => repo.FilterByAsync(x => x.AggregateIdentifier == aggregateId, CancellationToken.None))
                .ReturnsAsync(expectedEvents);

            // Act
            await eventStore.SaveEventsAsync(aggregateId, events, 1, CancellationToken.None);
            // Assert
            _eventProducerMock.Verify(producer => producer.ProduceAsync(It.IsAny<string>(), events[0]), Times.Once);
        }

        [Fact]
        public async Task SaveEventAsync_Should_ThrowException_WhenVersionMismatch()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            var aggregateId = Guid.NewGuid().ToString();
            var expectedEvents = new List<EventModel>
            {
                new EventModel { AggregateIdentifier = aggregateId, EventType = "TestEvent", Version = 2 }
            };
            var events = new List<BaseEvent>
            {
                new TicketCreatedEvent
                    { Version = 1, UserName = "test@email.com", TypeError = 0, DetailError = "detail" },
            };
            _eventModelRepositoryMock
                .Setup(repo => repo.FilterByAsync(x => x.AggregateIdentifier == aggregateId, CancellationToken.None))
                .ReturnsAsync(expectedEvents);

            // Act
            // Assert
            await Assert.ThrowsAsync<Exception>(() =>
                eventStore.SaveEventsAsync(aggregateId, events, 3, CancellationToken.None));
        }

        [Fact]
        public async Task AddEventStoreAsync_Should_AddEventStore()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            var aggregateId = Guid.NewGuid().ToString();
            var expectedEvent = new EventModel { AggregateIdentifier = aggregateId, EventType = "TestEvent" };
            var clientSessionHandleMock = Mock.Of<MongoDB.Driver.IClientSessionHandle>();
            _eventModelRepositoryMock
                .Setup(repo => repo.BeginSessionAsync(CancellationToken.None))
                .ReturnsAsync(clientSessionHandleMock);
            // Act
            await eventStore.AddEventStoreAsync(expectedEvent, CancellationToken.None);
            // Assert
            _eventModelRepositoryMock.Verify(repo => repo.BeginTransaction(clientSessionHandleMock), Times.Once);
            _eventModelRepositoryMock.Verify(
                repo => repo.InsertOneAsync(expectedEvent, clientSessionHandleMock, CancellationToken.None),
                Times.Once);
            _eventModelRepositoryMock.Verify(
                repo => repo.CommitTransactionAsync(clientSessionHandleMock, CancellationToken.None), Times.Once);
        }

        [Fact]
        public async Task AddEventStoreAsync_Should_ThrowException_WhenEventStoreIsNull()
        {
            // Arrange
            var eventStore = new EventStore(_eventModelRepositoryMock.Object, _kafkaSettingsMock,
                _eventProducerMock.Object);
            EventModel eventModel = null;
            _eventModelRepositoryMock
                .Setup(repo => repo.InsertOneAsync(eventModel, null, CancellationToken.None))
                .ThrowsAsync(new Exception());
            // Act
            await eventStore.AddEventStoreAsync(eventModel, CancellationToken.None);
            // Assert
            _eventModelRepositoryMock.Verify(
                repo => repo.RollbackTransactionAsync(null, CancellationToken.None), Times.Once);
        }
    }
}
