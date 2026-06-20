using Ardalis.ApiEndpoints;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Admin
{
    public class AdminVerificationActionRequest
    {
        [FromRoute] public string Id { get; set; } = null!;
        [FromRoute] public string Action { get; set; } = null!;
        [FromBody] public AdminVerificationBody? Body { get; set; }
    }

    public class AdminVerificationBody
    {
        public string? Reason { get; set; }
    }

    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminVerificationEndpoint : EndpointBaseAsync.WithRequest<AdminVerificationActionRequest>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public AdminVerificationEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("herders/{id}/{action}")]
        [SwaggerOperation(Summary = "Aprobar o rechazar verificación de cuidador", Tags = new[] { "Admin" })]
        public override async Task<ActionResult> HandleAsync(AdminVerificationActionRequest request, CancellationToken ct = default)
        {
            var herder = await _context.Herders.FindAsync(new object[] { request.Id }, ct);
            if (herder == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            switch (request.Action.ToLower())
            {
                case "approve":
                    herder.VerificationStatus = VerificationStatus.Verified;
                    herder.VerifiedAt = DateTime.UtcNow;
                    break;
                case "reject":
                    herder.VerificationStatus = VerificationStatus.Rejected;
                    herder.RejectionReason = request.Body?.Reason;
                    break;
                default:
                    return BadRequest(new APIResponse
                    {
                        StatusCode = HttpStatusCode.BadRequest,
                        IsSuccess = false,
                        ErrorMessages = new List<string> { "Acción no válida. Usa: approve o reject." }
                    });
            }

            await _context.SaveChangesAsync(ct);
            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = herder.VerificationStatus.ToString() });
        }
    }

    [Authorize(Roles = "Admin")]
    [Route("api/admin")]
    public class AdminListPendingHerdersEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public AdminListPendingHerdersEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet("herders/verification")]
        [SwaggerOperation(Summary = "Listar cuidadores pendientes de verificación", Tags = new[] { "Admin" })]
        public override async Task<ActionResult> HandleAsync(CancellationToken ct = default)
        {
            var herders = await _context.Herders
                .Where(h => h.VerificationStatus == VerificationStatus.UnderReview)
                .Select(h => new
                {
                    h.Id,
                    h.EmailUser,
                    h.VerificationStatus,
                    h.PhotoUrl,
                    h.IdDocumentFrontUrl,
                    h.IdDocumentBackUrl,
                    h.SelfieWithIdUrl,
                    h.City,
                    h.State
                })
                .ToListAsync(ct);

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = herders });
        }
    }
}
