using System.Text.Json;
using System.Text.Json.Serialization;
using Common.Core.Events;
using JsonException = Newtonsoft.Json.JsonException;

namespace Ticketing.Query.Infrastructure.Converters
{
    public class EventJsonConverter : JsonConverter<BaseEvent>
    {

        public override bool CanConvert(Type type)
        {
            return type.IsAssignableFrom(typeof(BaseEvent));
        }

        public override BaseEvent? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (!JsonDocument.TryParseValue(ref reader, out var doc))
            {
                throw new JsonException($"Failed to parse JSON for BaseEvent.{nameof(JsonDocument)}");
            }
            if(!doc.RootElement.TryGetProperty("Type", out var type))
            {
                throw new JsonException($"Failed to parse JSON for BaseEvent. Property 'Type'{nameof(type)}");
            }

            var typeDiscriminator = type.GetString();
            var json = doc.RootElement.GetRawText();

            return typeDiscriminator switch
            {
                nameof(TicketCreatedEvent) => JsonSerializer.Deserialize<TicketCreatedEvent>(json, options),
                nameof(TicketUpdatedEvent) => JsonSerializer.Deserialize<TicketUpdatedEvent>(json, options),
                nameof(TicketDeletedEvent) => JsonSerializer.Deserialize<TicketDeletedEvent>(json, options),
                _ => throw new JsonException($"Event type {typeDiscriminator} not implemented")
            };
        }

        public override void Write(Utf8JsonWriter writer, BaseEvent value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
