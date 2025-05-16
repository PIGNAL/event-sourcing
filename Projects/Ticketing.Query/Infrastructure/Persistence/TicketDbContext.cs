using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;

namespace Ticketing.Query.Infrastructure.Persistence
{
    public class TicketDbContext : DbContext
    {
        public virtual DbSet<Ticket> Tickets => Set<Ticket>();
        public virtual DbSet<Employee?> Employees => Set<Employee>();

        public TicketDbContext(DbContextOptions<TicketDbContext> options): base(options)
        {
            
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(TicketDbContext).Assembly);
            base.OnModelCreating(modelBuilder);
        }

    }
}
