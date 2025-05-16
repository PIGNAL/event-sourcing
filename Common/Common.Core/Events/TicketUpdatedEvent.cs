namespace Common.Core.Events
{
    public class TicketUpdatedEvent : BaseEvent
    {
        public int TicketType { get; set; }
        public string? Description { get; set; }
        public string? UserName { get; set; }

        public TicketUpdatedEvent() : base(nameof(TicketUpdatedEvent))
        {
        }
    }
}
