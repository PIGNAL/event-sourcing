using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Application.Extensions
{
    public static class MigrationExtension
    {
        public static async Task ApplyMigration(this IApplicationBuilder app)
        {
            using (var scope = app.ApplicationServices.CreateScope())
            {
                var service = scope.ServiceProvider;
                var loggerFactory = service.GetRequiredService<ILoggerFactory>();
                try
                {
                    var contextFactory = service.GetRequiredService<DataBaseContextFactory>();
                    using TicketDbContext dbContext = contextFactory.CreateDbContext();
                    await dbContext.Database.MigrateAsync();
                }
                catch (Exception ex)
                {
                    var logger = loggerFactory.CreateLogger<Program>();
                    logger.LogError(ex, "An error occurred while migrating the database used on context.");
                }
            }
        }
    }
}
