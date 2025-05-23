using Common.Core.Events;
using Ticketing.Command.Application.Aggregates;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Domain.Abstracts
{
    public class AggregateRootTests
    {
        [Fact]
        public void AggregateRoot_Constructor_SetsId()
        {
            // Arrange
            var id = Guid.NewGuid();
            var aggregateRoot = new TicketAggregate
            {
                Id = id.ToString()
            };
            // Act
            var result = aggregateRoot.Id;
            // Assert
            Assert.Equal(id.ToString(), result);
        }

        [Fact]
        public void AggregateRoot_Constructor_Id_IsNull()
        {
            // Arrange
            var aggregateRoot = new TicketAggregate();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => aggregateRoot.Id = null);
        }

        [Fact]
        public void AggregateRoot_Constructor_SetsVersion()
        {
            // Arrange
            var version = 1;
            var aggregateRoot = new TicketAggregate
            {
                Version = version
            };
            // Act
            var result = aggregateRoot.Version;
            // Assert
            Assert.Equal(version, result);
        }

        [Fact]
        public void AggregateRoot_MarkChangesAsCommitted()
        {
            // Arrange
            var aggregateRoot = new TicketAggregate();
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "TestUser",
                TypeError = 1,
                DetailError = "TestError"
            };
            aggregateRoot.RaiseEvent(ticketCreatedEvent);
            // Act
            aggregateRoot.MarkChangesAsCommitted();
            var changes = aggregateRoot.GetUncommittedChanges();
            // Assert
            Assert.Empty(changes);
        }

        [Fact]
        public void AggregateRoot_ApplyChanges_when_MethodIsNull()
        {
            // Arrange
            var aggregateRoot = new TicketAggregate();
            var testEvent = new TestEvent();
            // Act & Assert
            Assert.Throws<ArgumentNullException>(() => aggregateRoot.ApplyChange(testEvent, true));
        }

        private class TestEvent : BaseEvent
        {
            public TestEvent() : base(nameof(TestEvent))
            {
            }
        }

        [Fact]
        public void AggregateRoot_ReplayEvents_IsOk()
        {
            // Arrange
            var aggregateRoot = new TicketAggregate();
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "TestUser",
                TypeError = 1,
                DetailError = "TestError"
            };
            var events = new List<BaseEvent> { ticketCreatedEvent };
            // Act
            aggregateRoot.ReplayEvents(events);
            var changes = aggregateRoot.GetUncommittedChanges();
            // Assert
            Assert.Empty(changes);
        }
    }

    
}
