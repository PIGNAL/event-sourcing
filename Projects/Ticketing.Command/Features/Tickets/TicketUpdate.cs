using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets
{
    public class TicketUpdate : IMinimalApi
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPut("/api/ticket/{id}",
                async (
                    string id,
                    TicketUpdateRequest ticketUpdateRequest,
                    IMediator mediator,
                    CancellationToken cancellationToken) =>
            {
                var command = new TicketUpdateCommand(id, ticketUpdateRequest);
                var result = await mediator.Send(command, cancellationToken);
                return result ? Results.Ok(result) : Results.BadRequest();
            }).WithName("UpdateTicket");
        }

        public sealed class TicketUpdateRequest(int ticketType, string description, string userName)
        {
            public int TicketType { get; } = ticketType;
            public string Description { get; } = description;
            public string UserName { get; } = userName;

        }

        public record TicketUpdateCommand(string Id, TicketUpdateRequest TicketUpdateRequest) : IRequest<bool>;

        public class TicketUpdateCommandValidator : AbstractValidator<TicketUpdateCommand>
        {
            public TicketUpdateCommandValidator()
            {
                RuleFor(x => x.Id).NotEmpty();
                RuleFor(x => x.TicketUpdateRequest).SetValidator(new TicketUpdateRequestValidator());
            }


        }

        public class TicketUpdateRequestValidator : AbstractValidator<TicketUpdateRequest>
        {
            public TicketUpdateRequestValidator()
            {
                RuleFor(x => x.TicketType).NotEmpty().WithMessage("Add ticket type").InclusiveBetween(1, 5).WithMessage("Ticket type only can be 1 to 5");
                RuleFor(x => x.Description).NotEmpty().WithMessage("Add a description");
                RuleFor(x => x.UserName).NotEmpty().WithMessage("Add a userName");
            }
        }

        public sealed class TicketUpdateCommandHandler(IEventSourcingHandler<TicketAggregate> eventSourcingHandler) : IRequestHandler<TicketUpdateCommand, bool>
        {
            private readonly IEventSourcingHandler<TicketAggregate> _eventSourcingHandler = eventSourcingHandler;
            public async Task<bool> Handle(TicketUpdateCommand request, CancellationToken cancellationToken)
            {
                var aggregate = await _eventSourcingHandler.GetByIdAsync(request.Id, cancellationToken);
                aggregate.EditTicket(
                    request.TicketUpdateRequest.TicketType,
                    request.TicketUpdateRequest.Description,
                    request.TicketUpdateRequest.UserName);
                await _eventSourcingHandler.SaveAsync(aggregate, cancellationToken);

                return true;
            }
        }
    }
}
