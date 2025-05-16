using Common.Core.Events;

namespace Ticketing.Query.Domain.Abstractions
{
    public interface IEventHandler
    {
        Task On(TicketCreatedEvent @event);
        Task On(TicketUpdatedEvent @event);
    }
}
