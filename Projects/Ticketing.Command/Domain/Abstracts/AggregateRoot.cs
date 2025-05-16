using Common.Core.Events;

namespace Ticketing.Command.Domain.Abstracts
{
    public abstract class AggregateRoot
    {
        protected string _id = string.Empty;
        public string Id
        {
            get => _id;
            set
            {
                if (string.IsNullOrEmpty(value))
                {
                    throw new ArgumentNullException(nameof(Id), "Id cannot be null or empty.");
                }
                _id = value;
            }
        }
        public int Version { get; set; }
        public readonly List<BaseEvent>_changes = [];

        public IEnumerable<BaseEvent> GetUncommittedChanges()
        {
            return _changes;
        }

        public void MarkChangesAsCommitted()
        {
            _changes.Clear();
        }

        public void RaiseEvent(BaseEvent @event)
        {
            ApplyChange(@event, true);
        }

        public void ApplyChange(BaseEvent @event, bool isNewEvent)
        {
            var method = GetType().GetMethod("Apply", [@event.GetType()]);

            if (method is null)
            {
                throw new ArgumentNullException(nameof(method),
                    $"The Apply method was not found.{@event.GetType().Name}");
            }

            method.Invoke(this, [@event]);

            if (isNewEvent)
            {
                _changes.Add(@event);
            }
        }

        public void ReplayEvents(IEnumerable<BaseEvent> events)
        {
            foreach (var @event in events)
            {
                ApplyChange(@event, false);
            }
        }
    }
}
