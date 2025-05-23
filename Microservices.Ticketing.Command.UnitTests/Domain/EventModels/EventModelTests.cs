using Castle.Components.DictionaryAdapter;
using Common.Core.Events;
using Ticketing.Command.Domain.EventModels;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Domain.EventModels
{
    public class EventModelTests
    {
        [Fact]
        public void EventModel_SetAllProperties()
        {
            // Arrange
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = Guid.NewGuid().ToString(),
                UserName = "TestUser",
                TypeError = 1,
                DetailError = "TestError"
            };
            var eventModel = new EventModel
            {
                Id = MongoDB.Bson.ObjectId.GenerateNewId(),
                Timestamp = DateTime.UtcNow,
                AggregateIdentifier = Guid.NewGuid().ToString(),
                AggregateType = "TicketAggregate",
                Version = 2,
                EventType = "TicketCreated",
                EventData = ticketCreatedEvent,

            };
            // Act
            // Assert
            // Assert
            Assert.NotNull(eventModel.Id);
            Assert.True(eventModel.Timestamp <= DateTime.UtcNow && eventModel.Timestamp > DateTime.UtcNow.AddMinutes(-1));
            Assert.False(string.IsNullOrEmpty(eventModel.AggregateIdentifier));
            Assert.Equal("TicketAggregate", eventModel.AggregateType);
            Assert.Equal(2, eventModel.Version);
            Assert.Equal("TicketCreated", eventModel.EventType);
            Assert.NotNull(eventModel.EventData);

            var eventData = Assert.IsType<TicketCreatedEvent>(eventModel.EventData);
            Assert.Equal(ticketCreatedEvent.Id, eventData.Id);
            Assert.Equal(ticketCreatedEvent.UserName, eventData.UserName);
            Assert.Equal(ticketCreatedEvent.TypeError, eventData.TypeError);
            Assert.Equal(ticketCreatedEvent.DetailError, eventData.DetailError);

        }
    }
}
