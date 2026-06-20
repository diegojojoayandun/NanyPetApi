using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Appointments
{
    [Authorize]
    [Route("api/appointment")]
    public class ListAppointmentsEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ListAppointmentsEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Listar citas del usuario autenticado", Tags = new[] { "Citas" })]
        public override async Task<ActionResult> HandleAsync(CancellationToken ct = default)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (user == null) return Unauthorized();

            var roles = User.FindAll(ClaimTypes.Role).Select(c => c.Value).ToList();
            List<Appointment> appointments;

            if (roles.Contains("Herder"))
            {
                var herder = await _context.Herders.FirstOrDefaultAsync(h => h.EmailUser == user.UserName, ct);
                appointments = herder == null ? new() :
                    await _context.Appointments
                        .Where(a => a.HerderId == herder.Id)
                        .OrderByDescending(a => a.Created)
                        .ToListAsync(ct);
            }
            else
            {
                var owner = await _context.Owners.FirstOrDefaultAsync(o => o.EmailUser == user.Email, ct);
                appointments = owner == null ? new() :
                    await _context.Appointments
                        .Where(a => a.Pet!.OwnerId == owner.Id)
                        .OrderByDescending(a => a.Created)
                        .ToListAsync(ct);
            }

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<List<AppointmentDto>>(appointments)
            });
        }
    }
}
