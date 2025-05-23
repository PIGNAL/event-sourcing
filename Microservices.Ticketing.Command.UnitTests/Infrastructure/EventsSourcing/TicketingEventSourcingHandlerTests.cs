using Common.Core.Events;
using Moq;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;
using Ticketing.Command.Infrastructure.EventsSourcing;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Infrastructure.EventsSourcing
{
    public class TicketingEventSourcingHandlerTests
    {
        private readonly Mock<IEventStore> _eventStoreMock;
        private readonly TicketingEventSourcingHandler _handler;

        public TicketingEventSourcingHandlerTests()
        {
            _eventStoreMock = new Mock<IEventStore>();
            _handler = new TicketingEventSourcingHandler(_eventStoreMock.Object);
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsEmptyAggregate_WhenNoEvents()
        {
            // Arrange
            var aggregateId = "agg-1";
            _eventStoreMock.Setup(x => x.GetEventsAsync(aggregateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(new List<BaseEvent>());

            // Act
            var result = await _handler.GetByIdAsync(aggregateId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.IsType<TicketAggregate>(result);
            Assert.Equal(0, result.Version);
        }

        [Fact]
        public async Task GetByIdAsync_ReplaysEventsAndSetsVersion()
        {
            // Arrange
            var aggregateId = "agg-2";
            var events = new List<BaseEvent>
            {
                new TicketCreatedEvent { Version = 1, UserName = "user", TypeError = 0, DetailError = "detail" },
                new TicketUpdatedEvent { Version = 2, TicketType = 1, Description = "desc", UserName = "user" }
            };
            _eventStoreMock.Setup(x => x.GetEventsAsync(aggregateId, It.IsAny<CancellationToken>()))
                .ReturnsAsync(events);

            // Act
            var result = await _handler.GetByIdAsync(aggregateId, CancellationToken.None);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2, result.Version);
        }

        [Fact]
        public async Task SaveAsync_CallsEventStoreAndMarksChanges()
        {
            // Arrange
            var ticketCreateCommand = new TicketCreate.TicketCreateCommand(
                Guid.NewGuid().ToString(),
                new TicketCreate.TicketCreateRequest("test@email.com", 2, "detail0")
            );
            var aggregate = new TicketAggregate(ticketCreateCommand);
            var events = new List<BaseEvent>
            {
                new TicketCreatedEvent { Version = 1, UserName = "test@email.com", TypeError = 0, DetailError = "detail" },
                new TicketUpdatedEvent { Version = 2, TicketType = 1, Description = "desc", UserName = "test@email.com" }
            };
            aggregate.RaiseEvent(events[0]);
            aggregate.RaiseEvent(events[1]);
            // Act
            await _handler.SaveAsync(aggregate, CancellationToken.None);
            // Assert
            _eventStoreMock.Verify(x => x.SaveEventsAsync(aggregate.Id, aggregate.GetUncommittedChanges(), 0, It.IsAny<CancellationToken>()), Times.Once);
        }
    }

}
