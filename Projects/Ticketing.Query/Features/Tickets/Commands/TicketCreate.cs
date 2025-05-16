using MediatR;
using Ticketing.Query.Domain.Abstractions;
using Ticketing.Query.Domain.Employees;
using Ticketing.Query.Domain.Tickets;
using Ticketing.Query.Domain.TicketTypes;

namespace Ticketing.Query.Features.Tickets.Commands
{
    public sealed class TicketCreate
    {
        public record TicketCreateCommand(string Id, string Username, int TicketType, string DetailError)
            : IRequest<string>;

        public class TicketCreateCommandHandler : IRequestHandler<TicketCreateCommand, string>
        {
            private readonly IUnitOfWork _unitOfWork;
            public TicketCreateCommandHandler(IUnitOfWork unitOfWork)
            {
                _unitOfWork = unitOfWork;
            }
            public async Task<string> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
            {
                //1 insert data of employee
                var employee = await _unitOfWork.EmployeeRepository.GetByUsernameAsync(request.Username);
                if (employee == null)
                {
                    employee = Employee.Create(string.Empty, string.Empty, null!, request.Username);
                }
                _unitOfWork.EmployeeRepository.AddEntity(employee);

                //2 insert data of ticket
                var ticket = Ticket.Create(new Guid(request.Id), TicketType.Create(request.TicketType), request.DetailError);
                _unitOfWork.RepositoryGeneric<Ticket>().AddEntity(ticket);

                //3 insert data of ticket employee
                var ticketEmployee = TicketEmployee.Create(ticket, employee);
                _unitOfWork.RepositoryGeneric<TicketEmployee>().AddEntity(ticketEmployee);

                //4 save changes

                await _unitOfWork.Complete();

                return ticket.Id.ToString();
            }
        }
    }
}
