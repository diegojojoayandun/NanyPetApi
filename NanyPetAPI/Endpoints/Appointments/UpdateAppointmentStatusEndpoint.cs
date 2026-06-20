using Ardalis.ApiEndpoints;
using BusinessLogicLayer.Services.NotificationService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Appointments
{
    public class StatusUpdateRequest
    {
        [FromRoute] public string Id { get; set; } = null!;
        [FromRoute] public string Action { get; set; } = null!;
        [FromBody] public StatusUpdateBody? Body { get; set; }
    }

    public class StatusUpdateBody
    {
        public string? Reason { get; set; }
    }

    [Authorize]
    [Route("api/appointment")]
    public class UpdateAppointmentStatusEndpoint : EndpointBaseAsync.WithRequest<StatusUpdateRequest>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly INotificationService _notificationService;

        public UpdateAppointmentStatusEndpoint(ApplicationDbContext context, INotificationService notificationService)
        {
            _context = context;
            _notificationService = notificationService;
        }

        [HttpPut("{id}/{action}")]
        [SwaggerOperation(Summary = "Actualizar estado de cita (confirm/reject/start/complete/cancel)", Tags = new[] { "Citas" })]
        public override async Task<ActionResult> HandleAsync(StatusUpdateRequest request, CancellationToken ct = default)
        {
            var appointment = await _context.Appointments
                .Include(a => a.Pet).ThenInclude(p => p!.Owner)
                .Include(a => a.Herder)
                .FirstOrDefaultAsync(a => a.Id == request.Id, ct);

            if (appointment == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (user == null) return Unauthorized();

            string? ownerUserId = appointment.Pet?.Owner?.UserNameNavigation?.Id;
            string? herderUserId = appointment.Herder?.UserNameNavigation?.Id;

            switch (request.Action.ToLower())
            {
                case "confirm":
                    appointment.Status = AppointmentStatus.Confirmed;
                    if (ownerUserId != null)
                        await _notificationService.SendPushAsync(ownerUserId,
                            "Cita confirmada", "Tu cuidador confirmó la cita.",
                            NotificationType.AppointmentConfirmed, appointment.Id);
                    break;

                case "reject":
                    appointment.Status = AppointmentStatus.Rejected;
                    appointment.CancellationReason = request.Body?.Reason;
                    if (ownerUserId != null)
                        await _notificationService.SendPushAsync(ownerUserId,
                            "Cita rechazada", request.Body?.Reason ?? "El cuidador rechazó la cita.",
                            NotificationType.AppointmentRejected, appointment.Id);
                    break;

                case "start":
                    appointment.Status = AppointmentStatus.Active;
                    appointment.StartTime = DateTime.UtcNow;
                    if (ownerUserId != null)
                        await _notificationService.SendPushAsync(ownerUserId,
                            "Servicio iniciado", "El cuidador ya está con tu mascota.",
                            NotificationType.AppointmentActive, appointment.Id);
                    break;

                case "complete":
                    appointment.Status = AppointmentStatus.Completed;
                    appointment.EndTime = DateTime.UtcNow;
                    if (ownerUserId != null)
                        await _notificationService.SendPushAsync(ownerUserId,
                            "Servicio completado", "El cuidado de tu mascota ha finalizado.",
                            NotificationType.AppointmentCompleted, appointment.Id);
                    break;

                case "cancel":
                    appointment.Status = AppointmentStatus.Cancelled;
                    appointment.CancellationReason = request.Body?.Reason;
                    if (herderUserId != null)
                        await _notificationService.SendPushAsync(herderUserId,
                            "Cita cancelada", request.Body?.Reason ?? "El dueño canceló la cita.",
                            NotificationType.AppointmentRejected, appointment.Id);
                    break;

                default:
                    return BadRequest(new APIResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Acción no válida. Usa: confirm, reject, start, complete, cancel." }
                    });
            }

            appointment.LastModified = DateTime.UtcNow;
            await _context.SaveChangesAsync(ct);

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = appointment.Status.ToString() });
        }
    }
}
