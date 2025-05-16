using System.Text.Json;
using Common.Core.Consumers;
using Common.Core.Events;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Infrastructure.Converters;

namespace Ticketing.Query.Infrastructure.Consumers
{
    public class EventConsumer : IEventConsumer
    {
        private readonly ConsumerConfig _config;
        private readonly IServiceScopeFactory _serviceProvider;

        public EventConsumer(IOptions<ConsumerConfig> config, IServiceScopeFactory serviceProvider)
        {
            _config = config.Value;
            _serviceProvider = serviceProvider;
        }
        public void Consume(string topic)
        {
            using var consumer = new ConsumerBuilder<string, string>(_config)
                .SetKeyDeserializer(Deserializers.Utf8)
                .SetValueDeserializer(Deserializers.Utf8)
                .Build();

            consumer.Subscribe(topic);

            while (true)
            {
                var consumeResult = consumer.Consume();
                if (consumeResult is null) continue;
                if (consumeResult.Message is null) continue;

                var options = new JsonSerializerOptions { Converters = { new EventJsonConverter() } };
                var @event = JsonSerializer.Deserialize<BaseEvent>(consumeResult.Message.Value, options);
                if (@event is null)
                {
                    throw new ArgumentNullException(nameof(@event), "Failed to deserialize event");
                }

                var scope = _serviceProvider.CreateScope();
                var eventHandler = scope.ServiceProvider.GetRequiredService<IEventHandler>();
                var handlerMethod = eventHandler.GetType().GetMethod("On", [@event.GetType()]);
                if (handlerMethod is null)
                {
                    throw new ArgumentNullException(nameof(handlerMethod), "Failed to find event handler method");
                }
                handlerMethod.Invoke(eventHandler, [@event]);

                consumer.Commit(consumeResult);
            }
        }
    }
}
