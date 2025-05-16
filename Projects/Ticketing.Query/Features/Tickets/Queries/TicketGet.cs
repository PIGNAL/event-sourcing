using MediatR;
using Microsoft.EntityFrameworkCore;
using Ticketing.Query.Features.Tickets.DTOs;
using Ticketing.Query.Infrastructure.Persistence;

namespace Ticketing.Query.Features.Tickets.Queries
{
    public class TicketGet
    {
        public class TicketGetQuery : IRequest<List<TicketDto>>;

        public class TicketGetQueryHandler : IRequestHandler<TicketGetQuery, List<TicketDto>>
        {
            private readonly TicketDbContext _context;
            public TicketGetQueryHandler(TicketDbContext context)
            {
                _context = context;
            }

            public async Task<List<TicketDto>> Handle(TicketGetQuery request, CancellationToken cancellationToken)
            {
                var tickets = await _context.Tickets.ToListAsync(cancellationToken: cancellationToken);
                var ticketsDto = tickets.ConvertAll(t => t.ToDto());
                return ticketsDto;
            }
        }
    }
}
