using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands
{
    public sealed class TicketDelete
    {
        public record TicketDeleteCommand(string Id)
            : IRequest<string>;

        public class TicketDeleteCommandHandler : IRequestHandler<TicketDeleteCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;

            public TicketDeleteCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }

            public async Task<string> Handle(TicketDeleteCommand request, CancellationToken cancellationToken)
            {
                //1 search ticket
                var ticket = await _unitOfWork.RepositoryGeneric<Ticket>().GetByIdAsync(new Guid(request.Id));
                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                //2 delete data of ticket and sus references 
                _unitOfWork.RepositoryGeneric<Ticket>().DeleteEntity(ticket);


                await _unitOfWork.Complete();

                return ticket.Id.ToString();
            }
        }
    }
}
