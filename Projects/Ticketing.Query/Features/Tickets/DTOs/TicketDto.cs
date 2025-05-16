using Ticketing.Query.Domain.Tickets;

namespace Ticketing.Query.Features.Tickets.DTOs
{
    public static class TicketMapper
    {
        public static TicketDto ToDto(this Ticket ticket)
        {
            return new TicketDto(ticket.Id, ticket.Description, ticket.TicketType!.Id);
        }
    }
    public record TicketDto(Guid Id, string Description, int TicketType);
}
