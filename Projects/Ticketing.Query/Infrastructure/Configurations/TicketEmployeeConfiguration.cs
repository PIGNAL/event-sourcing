using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.Tickets;

namespace Ticketing.Query.Infrastructure.Configurations;

public class TicketEmployeeConfiguration : IEntityTypeConfiguration<TicketEmployee>
{
    public void Configure(EntityTypeBuilder<TicketEmployee> builder)
    {
        builder.ToTable("ticket_employees");
    }
}