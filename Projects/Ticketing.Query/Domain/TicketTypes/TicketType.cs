using System.Diagnostics.CodeAnalysis;
using Common.Core.Domain;

namespace Ticketing.Query.Domain.TicketTypes
{
    public class TicketType
    {
        private TicketType()
        {
        }

        [SetsRequiredMembers]
        private TicketType(int id, string name) => (Id, Name) = (id, name);

        public int Id { get; set; }
        public required string Name { get; set; }

        public static TicketType Create(int id)
        {
            var ticketTypeEnum = (TicketTypeEnum)id;
            var stringValue = ticketTypeEnum.ToString();
            return new TicketType(id, stringValue);
        }
    }
}
