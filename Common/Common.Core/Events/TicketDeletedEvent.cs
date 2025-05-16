namespace Common.Core.Events
{
    public class TicketDeletedEvent: BaseEvent 
    {
        public TicketDeletedEvent() : base(nameof(TicketDeletedEvent))
        {
        }
    }
}
