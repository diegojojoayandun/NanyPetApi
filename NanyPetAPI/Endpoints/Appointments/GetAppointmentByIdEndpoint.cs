using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Appointment;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Appointments
{
    [Authorize]
    [Route("api/appointment")]
    public class GetAppointmentByIdEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public GetAppointmentByIdEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetAppointment")]
        [SwaggerOperation(Summary = "Obtener cita por ID", Tags = new[] { "Citas" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { id }, ct);
            if (appointment == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = _mapper.Map<AppointmentDto>(appointment) });
        }
    }
}
