using Moq;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Features.Tickets
{
    public class TicketDeleteTests
    {

        [Fact]
        public async Task TicketDeleteCommandHandler_Handle_SavesAggregateAndReturnsTrue()
        {
            // Arrange
            var eventSourcingHandlerMock = new Mock<IEventSourcingHandler<TicketAggregate>>();
            eventSourcingHandlerMock
                .Setup(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            eventSourcingHandlerMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(new TicketAggregate()));
            var handler = new TicketDelete.TicketDeleteCommandHandler(eventSourcingHandlerMock.Object);
            var request = new TicketDelete.TicketDeleteCommand(Guid.NewGuid().ToString());

            // Act 
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result);
            eventSourcingHandlerMock.Verify(
                x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()), Times.Once);
        }


        [Fact]
        public void TicketDeleteValidator_ValidatesCorrectly()
        {
            // Arrange
            
            var id = Guid.NewGuid().ToString();
            var idEmpty = "";
            
            var validator = new TicketDelete.TicketDeleteCommandValidator();

            // Act
            var validResult = validator.Validate(new TicketDelete.TicketDeleteCommand(id));
            var invalidResult = validator.Validate(new TicketDelete.TicketDeleteCommand(idEmpty));

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(invalidResult.IsValid);
        }
    }
}
