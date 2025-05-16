using System.Numerics;
using System.Text.Json;
using Common.Core.Consumers;
using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Converters;

namespace Ticketing.Query.Infrastructure.Consumers
{
    public class ConsumerHostedService : IHostedService
    {
        private readonly ConsumerConfig _config;
        private readonly ILogger<ConsumerHostedService> _logger;
        private readonly IServiceProvider _serviceProvider;

        public ConsumerHostedService(IOptions<ConsumerConfig> config, ILogger<ConsumerHostedService> logger, IServiceProvider serviceProvider)
        {
            _config = config.Value;
            _logger = logger;
            _serviceProvider = serviceProvider;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting Kafka consumer service...");
            const string topic = "KAFKA_TOPIC";

            using (IServiceScope scope = _serviceProvider.CreateScope())
            {
                var eventConsumer = scope.ServiceProvider.GetRequiredService<IEventConsumer>();
                Task.Run(() => eventConsumer.Consume(topic), cancellationToken);

            }

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("Stopping Kafka consumer service...");
            return Task.CompletedTask;
        }
    }
}
