using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.NotificationService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Appointment;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Appointments
{
    [Authorize(Roles = "Owner")]
    [Route("api/appointment")]
    public class CreateAppointmentEndpoint : EndpointBaseAsync.WithRequest<AppointmentCreateDto>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly INotificationService _notificationService;

        public CreateAppointmentEndpoint(ApplicationDbContext context, IMapper mapper, INotificationService notificationService)
        {
            _context = context;
            _mapper = mapper;
            _notificationService = notificationService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Crear solicitud de cita", Tags = new[] { "Citas" })]
        public override async Task<ActionResult> HandleAsync(AppointmentCreateDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var herder = await _context.Herders.FindAsync(new object[] { request.HerderId }, ct);
            if (herder == null || herder.VerificationStatus != VerificationStatus.Verified)
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "El cuidador no está verificado." }
                });

            var appointment = new Appointment
            {
                PetId = request.PetId,
                HerderId = request.HerderId,
                Status = AppointmentStatus.PendingRequest,
                Price = request.Price,
                AppointmentTime = request.AppointmentTime,
                ServiceType = request.ServiceType,
                Notes = request.Notes,
                SpecialRequirements = request.SpecialRequirements,
                Created = DateTime.UtcNow
            };

            _context.Appointments.Add(appointment);
            await _context.SaveChangesAsync(ct);

            // Notificar al herder
            var herderUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == herder.EmailUser, ct);
            if (herderUser != null)
                await _notificationService.SendPushAsync(herderUser.Id,
                    "Nueva solicitud de cita",
                    $"Tienes una nueva solicitud para el {request.AppointmentTime:dd/MM/yyyy HH:mm}",
                    NotificationType.AppointmentRequest, appointment.Id);

            return CreatedAtRoute("GetAppointment", new { id = appointment.Id }, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Result = _mapper.Map<AppointmentDto>(appointment)
            });
        }
    }
}
