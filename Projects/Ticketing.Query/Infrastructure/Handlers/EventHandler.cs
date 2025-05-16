using Common.Core.Events;
using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Features.Tickets.Commands;

namespace Ticketing.Query.Infrastructure.Handlers
{
    public class EventHandler : IEventHandler
    {
        private readonly IMediator _mediator;
        public EventHandler(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task On(TicketCreatedEvent @event)
        {
            var ticketCreateCommand = new TicketCreate.TicketCreateCommand(
                @event.Id,
                @event.UserName,
                @event.TypeError,
                @event.DetailError);

            await _mediator.Send(ticketCreateCommand);
        }

        public async Task On(TicketUpdatedEvent @event)
        {
            var ticketUpdatedEvent = new TicketUpdate.TicketUpdateCommand(
                @event.Id,
                @event.UserName!,
                @event.TicketType,
                @event.Description!);
            await _mediator.Send(ticketUpdatedEvent);
        }
    }
}
