using Moq;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Features.Tickets
{
    public class TicketCreateTests
    {

        [Fact]
        public async Task TicketCreateCommandHandler_Handle_SavesAggregateAndReturnsTrue()
        {
            // Arrange
            var eventSourcingHandlerMock = new Mock<IEventSourcingHandler<TicketAggregate>>();
            eventSourcingHandlerMock
                .Setup(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);

            var handler = new TicketCreate.TicketCreateCommandHandler(eventSourcingHandlerMock.Object);
            var request = new TicketCreate.TicketCreateCommand(
                Guid.NewGuid().ToString(),
                new TicketCreate.TicketCreateRequest("test@email.com", 1, "error test")
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result);
            eventSourcingHandlerMock.Verify(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TicketCreateCommandValidator_ValidatesCorrectly()
        {
            // Arrange
            var validator = new TicketCreate.TicketCreateCommand.TicketCreateCommandValidator();
            var validRequest = new TicketCreate.TicketCreateCommand(
                Guid.NewGuid().ToString(),
                new TicketCreate.TicketCreateRequest("test@email.com", 2, "detail")
            );
            var invalidRequest = new TicketCreate.TicketCreateCommand(
                "",
                new TicketCreate.TicketCreateRequest("", 0, "")
            );

            // Act
            var validResult = validator.Validate(validRequest);
            var invalidResult = validator.Validate(invalidRequest);

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(invalidResult.IsValid);
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "Id");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("UserName"));
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("TypeError"));
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("DetailError"));
        }

        [Fact]
        public void TicketCreateValidator_ValidatesCorrectly()
        {
            // Arrange
            var validator = new TicketCreate.TicketCreateValidator();
            var validRequest = new TicketCreate.TicketCreateRequest("test@email.com", 3, "detail");
            var invalidRequest = new TicketCreate.TicketCreateRequest("", 0, "");

            // Act
            var validResult = validator.Validate(validRequest);
            var invalidResult = validator.Validate(invalidRequest);

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(invalidResult.IsValid);
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "UserName");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "TypeError");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "DetailError");
        }

        [Fact]
        public void TicketCreateRequest_Should_Constructor()
        {
            // Arrange
            var userName = "joniballatore@gmail.com";
            var typeError = 5;
            var detailError = "red error";
            var ticketCreateRequest = new TicketCreate.TicketCreateRequest(userName, typeError, detailError);

            // Act & Assert
            Assert.Equal(userName, ticketCreateRequest.UserName);
            Assert.Equal(typeError, ticketCreateRequest.TypeError);
            Assert.Equal(detailError, ticketCreateRequest.DetailError);
        }
    }
}
