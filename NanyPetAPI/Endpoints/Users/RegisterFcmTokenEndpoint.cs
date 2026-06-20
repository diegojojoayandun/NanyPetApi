using Ardalis.ApiEndpoints;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Users
{
    public class FcmTokenRequest
    {
        public string Token { get; set; } = null!;
    }

    [Authorize]
    [Route("api/user")]
    public class RegisterFcmTokenEndpoint : EndpointBaseAsync.WithRequest<FcmTokenRequest>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public RegisterFcmTokenEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("fcm-token")]
        [SwaggerOperation(Summary = "Registrar token FCM para notificaciones push", Tags = new[] { "Usuarios" })]
        public override async Task<ActionResult> HandleAsync(FcmTokenRequest request, CancellationToken ct = default)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (user == null) return Unauthorized();

            user.FcmToken = request.Token;
            await _context.SaveChangesAsync(ct);

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK });
        }
    }
}
