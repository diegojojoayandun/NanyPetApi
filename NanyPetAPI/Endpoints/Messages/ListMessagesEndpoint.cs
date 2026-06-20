using Ardalis.ApiEndpoints;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Messages
{
    [Authorize]
    [Route("api/appointment")]
    public class ListMessagesEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public ListMessagesEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("{id}/messages")]
        [SwaggerOperation(Summary = "Historial de mensajes de una cita", Tags = new[] { "Chat" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var messages = await _context.Messages
                .Where(m => m.AppointmentId == id)
                .OrderBy(m => m.SentAt)
                .Select(m => new
                {
                    m.Id,
                    m.AppointmentId,
                    m.SenderId,
                    senderName = m.Sender.FirstName + " " + m.Sender.LastName,
                    m.Content,
                    m.SentAt,
                    m.IsRead
                })
                .ToListAsync(ct);

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = messages });
        }
    }
}
