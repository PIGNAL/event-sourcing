using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Domain.Tickets
{
    public class Ticket : Entity
    {
        public string? Description { get; set; }
        public virtual TicketType? TicketType { get; set; }
        public virtual ICollection<Employee> Employees { get; set; } = [];
        public virtual ICollection<TicketEmployee> TicketEmployees { get; set; } = [];

        public Ticket()
        {

        }

        private Ticket(Guid id, TicketType? ticketType, string description) : base(id)
        {
            TicketType = ticketType;
            Description = description;
        }

        public static Ticket Create(Guid id, TicketType? ticketType, string description)
        {
            return new Ticket(id, ticketType, description);
        }
    }
}
