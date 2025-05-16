using FluentValidation;
using MediatR;
using Ticketing.Command.Application.Aggregates;
using Ticketing.Command.Domain.Abstracts;
using Ticketing.Command.Features.Apis;

namespace Ticketing.Command.Features.Tickets
{
    public sealed class TicketCreate : IMinimalApi
    {
        public void AddEndpoint(IEndpointRouteBuilder endpointRouteBuilder)
        {
            endpointRouteBuilder.MapPost("/api/ticket", async (TicketCreateRequest ticketCreateRequest, IMediator mediator, CancellationToken cancellationToken) =>
            {
                var id = Guid.CreateVersion7(DateTimeOffset.UtcNow).ToString();
                var command = new TicketCreateCommand(id, ticketCreateRequest);
                var result = await mediator.Send(command, cancellationToken);
                return result ? Results.Ok(result) : Results.BadRequest();
            }).WithName("CreateTicket");
        }

        public sealed class TicketCreateRequest(string userName, int typeError, string detailError)
        {
            public string UserName { get; set; } = userName;
            public int TypeError { get; set; } = typeError;
            public string DetailError { get; set; } = detailError;
        }

        public record TicketCreateCommand(string Id, TicketCreateRequest TicketCreateRequest) : IRequest<bool>
        {
            public class TicketCreateCommandValidator : AbstractValidator<TicketCreateCommand>
            {
                public TicketCreateCommandValidator()
                {
                    RuleFor(x => x.TicketCreateRequest).SetValidator(new TicketCreateValidator());
                    RuleFor(x => x.Id)
                        .NotEmpty()
                        .WithMessage("Id is required.");
                }
            }
        }

        public class TicketCreateValidator : AbstractValidator<TicketCreateRequest>
        {
            public TicketCreateValidator()
            {
                RuleFor(x => x.UserName)
                    .NotEmpty()
                    .WithMessage("UserName is required.")
                    .EmailAddress()
                    .WithMessage("Need a email");
                RuleFor(x => x.TypeError)
                    .NotEmpty()
                    .WithMessage("TypeError is required.")
                    .InclusiveBetween(1, 5)
                    .WithMessage("Range is 1 to 5");

                RuleFor(x => x.DetailError)
                    .NotEmpty()
                    .WithMessage("DetailError is required.");
            }
        }

        public sealed class TicketCreateCommandHandler(IEventSourcingHandler<TicketAggregate> eventSourcingHandler) : IRequestHandler<TicketCreateCommand, bool>
        {
            public async Task<bool> Handle(TicketCreateCommand request, CancellationToken cancellationToken)
            {
                var aggregate = new TicketAggregate(request);
                await eventSourcingHandler.SaveAsync(aggregate, cancellationToken);
                return true;
            }
        }
    }
}
