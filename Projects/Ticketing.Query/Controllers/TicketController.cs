using MediatR;
using Microsoft.AspNetCore.Mvc;
using Ticketing.Query.Features.Tickets.DTOs;
using Ticketing.Query.Features.Tickets.Queries;

namespace Ticketing.Query.Controllers
{
    [ApiController]
    [Route("api/tickets")]
    public class TicketController : ControllerBase
    {
        private readonly IMediator _mediator;
        public TicketController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> Get()
        {
            var query = new TicketGet.TicketGetQuery();
            var results = await _mediator.Send(query);
            
            return Ok(results);
        }
    }
}
