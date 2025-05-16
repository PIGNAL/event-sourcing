using Common.Core.Messages;

namespace Common.Core.Events
{
    public abstract class BaseEvent : Message
    {
        public int Version { get; set; }
        public string Type { get; set; }

        protected BaseEvent(string type)
        {
            Type = type;
        }
    }
}
