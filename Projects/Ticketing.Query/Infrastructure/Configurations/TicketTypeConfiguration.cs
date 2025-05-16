using Common.Core.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infrastructure.Configurations
{
    public class TicketTypeConfiguration: IEntityTypeConfiguration<TicketType>
    {
        public void Configure(EntityTypeBuilder<TicketType> builder)
        {
            builder.ToTable("ticket_types");
            builder.HasKey(t => t.Id);

            var ticketTypes = Enum.GetValues<TicketTypeEnum>()
                .Select(tte => TicketType.Create((int)tte));

            builder.HasData(ticketTypes);
        }
    }
}
