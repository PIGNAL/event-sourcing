using Microsoft.EntityFrameworkCore;

namespace Ticketing.Query.Infrastructure.Persistence
{
    public class DataBaseContextFactory
    {
        private readonly Action<DbContextOptionsBuilder> _configureDbContext;

        public DataBaseContextFactory(Action<DbContextOptionsBuilder> configureDbContext)
        {
            _configureDbContext = configureDbContext;
        }

        public TicketDbContext CreateDbContext()
        {
            var optionsBuilder = new DbContextOptionsBuilder<TicketDbContext>();
            _configureDbContext(optionsBuilder);
            return new TicketDbContext(optionsBuilder.Options);
        }
    }
}
