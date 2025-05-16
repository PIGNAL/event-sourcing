using Common.Core.Events;
using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets
{
    public class TicketDelete :IMinimalApi
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapDelete("/api/ticket/{id}",
                async (
                    string id,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
                {
                    var command = new TicketDeleteCommand(id);
                    var result = await mediator.Send(command, cancellationToken);
                    return result ? Results.Ok(result) : Results.BadRequest();
                }).WithName("DeleteTicket");
        }

        public record TicketDeleteCommand(string Id) : IRequest<bool>;

        public class TicketDeleteCommandValidator : AbstractValidator<TicketDeleteCommand>
        {
            public TicketDeleteCommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
            }
        }

        public sealed class TicketDeleteCommandHandler(IEventSourcingHandler<TicketAggregate> eventSourcingHandler) : IRequestHandler<TicketDeleteCommand, bool>
        {
            private readonly IEventSourcingHandler<TicketAggregate> _eventSourcingHandler = eventSourcingHandler;

            public async Task<bool> Handle(TicketDeleteCommand request, CancellationToken cancellationToken)
            {
                var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id, cancellationToken);
                var ticketDeletedEvent = new TicketDeletedEvent
                {
                    Id = request.Id
                };

                aggregate.RaiseEvent(ticketDeletedEvent);
                await _eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
                return true;
            }
        }
    }
}
