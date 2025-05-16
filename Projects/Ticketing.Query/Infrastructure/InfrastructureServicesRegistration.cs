using Common.Core.Consumers;
using Confluent.Kafka;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Infrastructure.Consumers;
using Ticketing.Query.Infrastructure.Persistence;
using Ticketing.Query.Infrastructure.Persistence.Interceptors;
using Ticketing.Query.Infrastructure.Repositories;

namespace Ticketing.Query.Infrastructure
{
    public static class InfrastructureServicesRegistration
    {
        public static IServiceCollection RegisterInfrastructureServices(this IServiceCollection services,
            IConfiguration configuration)
        {
            Action<DbContextOptionsBuilder> configureDbContext;

            services.AddSingleton<AuditEntitiesInterceptor>();

            var connectionString = configuration.GetConnectionString("PostgresConnectionString")
                                   ?? throw new ArgumentNullException(nameof(configuration));
            configureDbContext = o => o
                .UseLazyLoadingProxies()
                .UseNpgsql(connectionString)
                .UseSnakeCaseNamingConvention()
                .AddInterceptors(new AuditEntitiesInterceptor());

            services.AddDbContext<TicketDbContext>(opt =>
            {
                opt.UseNpgsql(connectionString).UseSnakeCaseNamingConvention();
            });
            services.AddSingleton(new DataBaseContextFactory(configureDbContext));
            
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));

            services.Configure<ConsumerConfig>(configuration.GetSection(nameof(ConsumerConfig)));

            services.AddScoped<IEmployeeRepository, EmployeeRepository>();
            services.AddScoped<IEventConsumer, EventConsumer>();

            services.AddHostedService<ConsumerHostedService>();
            services.AddScoped<IEventHandler, Handlers.EventHandler>();

            
            return services;
        }
    }
}
