using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Infrastructure.Configurations
{
    public class TicketConfiguration : IEntityTypeConfiguration<Ticket>
    {
        public void Configure(EntityTypeBuilder<Ticket> builder)
        {
            builder.ToTable("tickets");
            builder.HasKey(t => t.Id);
            builder.Property(t => t.TicketType).HasConversion(tp => tp!.Id, value => TicketType.Create(value));
            builder.HasMany(t=>t.Employees).WithMany(e => e.Tickets)
                .UsingEntity<TicketEmployee>(
                    j => j
                        .HasOne(te => te.Employee)
                        .WithMany(e => e.TicketEmployees)
                        .HasForeignKey(te => te.EmployeeId),
                    j => j
                        .HasOne(te => te.Ticket)
                        .WithMany(t => t.TicketEmployees)
                        .HasForeignKey(te => te.TicketId),
                    j =>
                    {
                        j.HasKey(te => new { te.TicketId, te.EmployeeId });
                    });
        }
    }
}
