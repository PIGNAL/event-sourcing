using Common.Core.Events;
using Common.Core.Producers;
using Microsoft.Extensions.Options;
using Ticketing.Command.Application.Models;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Domain.EventModels;

namespace Ticketing.Command.Infrastructure.Persistence
{
    public class EventStore : IEventStore
    {
        private readonly IEventModelRepository _eventModelRepository;
        private readonly KafkaSettings _kafkaSettings;
        private readonly IEventProducer _eventProducer;

        public EventStore(IEventModelRepository eventModelRepository, IOptions<KafkaSettings> kafkaSettings, IEventProducer eventProducer)
        {
            _eventModelRepository = eventModelRepository;
            _kafkaSettings = kafkaSettings.Value;
            _eventProducer = eventProducer;
        }

        public async Task<List<BaseEvent>> GetEventsAsync(string aggregateId, CancellationToken cancellationToken)
        {
            var eventStream =
                await _eventModelRepository.FilterByAsync(doc => doc.AggregateIdentifier == aggregateId,
                    cancellationToken);
            if (eventStream is null || !eventStream.Any())
            {
                throw new Exception("eventStream is null");
            }

            return eventStream.OrderBy(x => x.Version).Select(x => x.EventData).ToList();
        }

        public async Task SaveEventsAsync(string aggregateId, IEnumerable<BaseEvent> events, int expectedVersion, CancellationToken cancellationToken)
        {
            var eventStream = await _eventModelRepository.FilterByAsync(doc => doc.AggregateIdentifier == aggregateId,
                cancellationToken);
            if (eventStream.Any() && expectedVersion != -1 && eventStream.Last().Version != expectedVersion)
            {
                throw new Exception("Concurrence Error");
            }

            var version = expectedVersion;
            foreach (var @event in events)
            {
                version++;
                @event.Version = version;
                var eventType = @event.GetType().Name;
                var eventModel = new EventModel
                {
                    Timestamp = DateTime.UtcNow,
                    AggregateIdentifier = aggregateId,
                    AggregateType = "TicketAggregate",
                    Version = version,
                    EventType = eventType,
                    EventData = @event,
                };
                var topic = _kafkaSettings.Topic?? throw new Exception("Kafka topic is not configured in KafkaSettings.");
                await AddEventStore(eventModel, cancellationToken);
                await _eventProducer.ProduceAsync(topic, @event);
            }
        }

        private async Task AddEventStore(EventModel eventModel, CancellationToken cancellationToken)
        {
            var session = await _eventModelRepository.BeginSessionAsync(cancellationToken);
            try
            {
                _eventModelRepository.BeginTransaction(session);
                await _eventModelRepository.InsertOneAsync(eventModel, session, cancellationToken);
                await _eventModelRepository.CommitTransactionAsync(session, cancellationToken);
            }
            catch (Exception)
            {
                await _eventModelRepository.RollbackTransactionAsync(session, cancellationToken);
            }
            finally
            {
                _eventModelRepository.DisposeSession(session);
            }
        }
    }
}
