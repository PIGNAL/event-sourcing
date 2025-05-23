using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Routing;
using Moq;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;
using Xunit;

namespace Microservices.Ticketing.Command.UnitTests.Features.Tickets
{
    public class TicketUpdateTests
    {
        //[Fact]
        //public void TicketUpdate_AddEndpoint_Should()
        //{
        //}

        [Fact]
        public async Task TicketUpdateCommandHandler_Handle_SavesAggregateAndReturnsTrue()
        {
            // Arrange
            var eventSourcingHandlerMock = new Mock<IEventSourcingHandler<TicketAggregate>>();
            eventSourcingHandlerMock
                .Setup(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask);
            var ticketAggregate = new TicketAggregate
            {
                Active = true
            };
            eventSourcingHandlerMock.Setup(x => x.GetByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .Returns(Task.FromResult(ticketAggregate));
            var handler = new TicketUpdate.TicketUpdateCommandHandler(eventSourcingHandlerMock.Object);
            var request = new TicketUpdate.TicketUpdateCommand(
                Guid.NewGuid().ToString(),
                new TicketUpdate.TicketUpdateRequest(3, "detail", "test@email.com")
            );

            // Act
            var result = await handler.Handle(request, CancellationToken.None);

            // Assert
            Assert.True(result);
            eventSourcingHandlerMock.Verify(x => x.SaveAsync(It.IsAny<TicketAggregate>(), It.IsAny<CancellationToken>()), Times.Once);
        }

        [Fact]
        public void TicketUpdateCommandValidator_ValidatesCorrectly()
        {
            // Arrange
            var validator = new TicketUpdate.TicketUpdateCommandValidator();
            var validRequest = new TicketUpdate.TicketUpdateCommand(
                Guid.NewGuid().ToString(),
                new TicketUpdate.TicketUpdateRequest(3, "detail", "test@email.com")
            );
            var invalidRequest = new TicketUpdate.TicketUpdateCommand(
                "",
                new TicketUpdate.TicketUpdateRequest(0, "", "")
            );

            // Act
            var validResult = validator.Validate(validRequest);
            var invalidResult = validator.Validate(invalidRequest);

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(invalidResult.IsValid);
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "Id");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("UserName"));
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("TicketType"));
            Assert.Contains(invalidResult.Errors, e => e.PropertyName.Contains("Description"));
        }

        [Fact]
        public void TicketUpdateValidator_ValidatesCorrectly()
        {
            // Arrange
            var validator = new TicketUpdate.TicketUpdateRequestValidator();
            var validRequest = new TicketUpdate.TicketUpdateRequest(3, "detail", "test@email.com");
            var invalidRequest = new TicketUpdate.TicketUpdateRequest(0, "", "");

            // Act
            var validResult = validator.Validate(validRequest);
            var invalidResult = validator.Validate(invalidRequest);

            // Assert
            Assert.True(validResult.IsValid);
            Assert.False(invalidResult.IsValid);
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "UserName");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "TicketType");
            Assert.Contains(invalidResult.Errors, e => e.PropertyName == "Description");
        }

        [Fact]
        public void TicketUpdateRequest_Should_Constructor()
        {
            // Arrange
            var ticketType = 3;
            var userName = "joniballatore@gmail.com";
            var description = "red error";
            var ticketCreateRequest = new TicketUpdate.TicketUpdateRequest(ticketType, description, userName);

            // Act & Assert
            Assert.Equal(userName, ticketCreateRequest.UserName);
            Assert.Equal(ticketType, ticketCreateRequest.TicketType);
            Assert.Equal(description, ticketCreateRequest.Description);
        }
    }

}
