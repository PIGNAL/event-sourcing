using Common.Core.Events;
using Moq;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;
using Xunit;

namespace Tests.Ticketing.Command.Application.Aggregates
{

    public class TicketAggregateTests
    {
        // Test data
        const string UserName = "joniballatore@gmail.com";
        const int TypeError = 1;
        const string DetailError = "because connexion is bad";
        [Fact]
        public void TestTicketCreationWitchCreateEvent()
        {

            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);

            Assert.NotNull(ticketAggregate);
            Assert.True(ticketAggregate.Active);
            Assert.Equal(ticketCreateCommand.Id, ticketAggregate.Id);


        }

        [Fact]
        public void TestGetUncommittedChanges()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest("user", 1, "detail");
            var ticketCreateCommand = new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);

            // Act
            var aggregate = new TicketAggregate(ticketCreateCommand);

            // Assert
            var changes = aggregate.GetUncommittedChanges().ToList();
            Assert.Single(changes);
            Assert.IsType<TicketCreatedEvent>(changes[0]);
        }

        [Fact]
        public void TestTicketCreationWitchCreateEventWithMock()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var eventSourcingHandlerMock = new Mock<IEventSourcingHandler<TicketAggregate>>();
            eventSourcingHandlerMock
                .Setup<Task>(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            // Act
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);

            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.True(ticketAggregate.Active);
            Assert.Equal(ticketCreateCommand.Id, ticketAggregate.Id);
        }

        [Fact]
        public void TestTicketAggregateWithTicketDeleteCommand()
        {
            // Arrange
            var ticketDeleteCommand = new TicketDelete.TicketDeleteCommand(Guid.NewGuid().ToString());
            var ticketAggregate = new TicketAggregate(ticketDeleteCommand);
            // Act
            var ticketDeletedEvent = new TicketDeletedEvent
            {
                Id = ticketDeleteCommand.Id
            };
            ticketAggregate.RaiseEvent(ticketDeletedEvent);
            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.False(ticketAggregate.Active);
            Assert.Equal(ticketDeleteCommand.Id, ticketAggregate.Id);
        }

        [Fact]
        public void TestEditTicket()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);
            // Act
            ticketAggregate.EditTicket(2, "New description", UserName);
            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.True(ticketAggregate.Active);
        }

        [Fact]
        public void TestEditTicketWithInvalidState()
        {
            // Arrange
            var ticketDeleteCommand = new TicketDelete.TicketDeleteCommand(Guid.NewGuid().ToString());
            var ticketAggregate = new TicketAggregate(ticketDeleteCommand);
            // Act & Assert
            Assert.Throws<InvalidOperationException>(() => ticketAggregate.EditTicket(2, "New description", UserName));
            Assert.False(ticketAggregate.Active);
        }

        [Fact]
        public void TestApplyTicketCreateEvent()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = ticketAggregate.Id,
                TypeError = TypeError,
                DetailError = DetailError,
                UserName = UserName
            };
            // Act
            ticketAggregate.Apply(ticketCreatedEvent);
            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.True(ticketAggregate.Active);
        }


        [Fact]
        public void TestApplyTicketUpdateEvent()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);
            var ticketUpdateEvent = new TicketUpdatedEvent
            {
                Id = ticketAggregate.Id,
                TicketType = 2,
                Description = "New description",
                UserName = UserName
            };
            // Act
            ticketAggregate.Apply(ticketUpdateEvent);
            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.True(ticketAggregate.Active);
        }

        [Fact]
        public void TestApplyTicketDeleteEvent()
        {
            // Arrange
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(UserName, TypeError, DetailError);
            var ticketCreateCommand =
                new TicketCreate.TicketCreateCommand(Guid.NewGuid().ToString(), ticketCreateRequest);
            var ticketAggregate = new TicketAggregate(ticketCreateCommand);
            var ticketDeleteEvent = new TicketDeletedEvent
            {
                Id = ticketAggregate.Id
            };
            // Act
            ticketAggregate.Apply(ticketDeleteEvent);
            // Assert
            Assert.NotNull(ticketAggregate);
            Assert.False(ticketAggregate.Active);
        }

    }
}
