using Common.Core.Events;
using Common.Core.Producers;
using Confluent.Kafka;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Ticketing.Command.Application.Models;

namespace Ticketing.Command.Infrastructure.Persistence
{
    public class TicketEventProducer: IEventProducer
    {
        private readonly KafkaSettings _kafkaSettings;

        public TicketEventProducer(IOptions<KafkaSettings> kafkaSettings)
        {
            _kafkaSettings = kafkaSettings.Value;
        }

        public async Task ProduceAsync(string topic, BaseEvent @event)
        {
            var config = new ProducerConfig
            {
                BootstrapServers = $"{_kafkaSettings.Hostname}:{_kafkaSettings.Port}"
            };

            using var producer = new ProducerBuilder<string, string>(config)
                                .SetKeySerializer(Serializers.Utf8)
                                .SetValueSerializer(Serializers.Utf8)
                                .Build();
            var eventMessage = new Message<string, string>
            {
                Key = Guid.NewGuid().ToString(),
                Value = JsonConvert.SerializeObject(@event)
            };

            var deliveryStatus = await producer.ProduceAsync(topic, eventMessage);
            if (deliveryStatus.Status == PersistenceStatus.NotPersisted)
            {
                throw new Exception(
                    $"Failed to deliver message {@event.GetType().Name} to topic {topic}, for this {deliveryStatus.Message} ");
            }
        }
    }
}
