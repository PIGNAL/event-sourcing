using Common.Core.Events;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Tickets;

namespace Ticketing.Command.Application.Aggregates
{
    public class TicketAggregate : AggregateRoot
    {
        public bool Active { get; set; }

        public TicketAggregate()
        {

        }

        public TicketAggregate(TicketCreate.TicketCreateCommand command)
        {
            var ticketCreatedEvent = new TicketCreatedEvent
            {
                Id = command.Id,
                UserName = command.TicketCreateRequest.UserName,
                TypeError = command.TicketCreateRequest.TypeError,
                DetailError = command.TicketCreateRequest.DetailError
            };

            RaiseEvent(ticketCreatedEvent);
        }

        public TicketAggregate(TicketDelete.TicketDeleteCommand command)
        {
            var ticketDeletedEvent = new TicketDeletedEvent
            {
                Id = command.Id
            };

            RaiseEvent(ticketDeletedEvent);
        }

    public void Apply(TicketCreatedEvent @event)
        {
            _id = @event.Id;
            Active = true;
        }

        public void EditTicket(int ticketType, string description, string userName)
        {
            if (!Active)
            { 
                throw new InvalidOperationException("Cannot edit an inactive ticket.");
            }

            RaiseEvent(new TicketUpdatedEvent
            {
                Id = Id,
                TicketType = ticketType,
                Description = description,
                UserName = userName
            });
        }

        public void Apply(TicketUpdatedEvent @event)
        {
            _id = @event.Id;
        }

        public void Apply(TicketDeletedEvent @event)
        {
            _id = @event.Id;
            Active = false;
        }
    }
}
