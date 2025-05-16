using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands
{
    public sealed class TicketUpdate
    {
        public record TicketUpdateCommand(string Id, string UserName, int TicketType, string Description)
            : IRequest<string>;

        public class TicketUpdateCommandHandler : IRequestHandler<TicketUpdateCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;
            public TicketUpdateCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<string> Handle(TicketUpdateCommand request, CancellationToken cancellationToken)
            {
                //1 update data of ticket
                var ticket = await _unitOfWork.RepositoryGeneric<Ticket>().GetByIdAsync(new Guid(request.Id));
                if (ticket == null)
                {
                    throw new Exception("Ticket not found");
                }
                ticket.Description = request.Description;
                ticket.TicketType = TicketType.Create(request.TicketType);
                //2 insert data of employee
                var employee = await _unitOfWork.EmployeeRepository.GetByUsernameAsync(request.UserName);
                if (employee == null)
                {
                    employee = Employee.Create(string.Empty, string.Empty, null!, request.UserName);
                }
                _unitOfWork.EmployeeRepository.AddEntity(employee);

                //3 save changes

                await _unitOfWork.Complete();

                return ticket.Id.ToString();
            }
        }
    }
}
