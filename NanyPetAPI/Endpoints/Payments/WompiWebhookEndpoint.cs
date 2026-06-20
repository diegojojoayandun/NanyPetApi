using Ardalis.ApiEndpoints;
using BusinessLogicLayer.Services.NotificationService;
using BusinessLogicLayer.Services.PaymentService;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace NanyPetAPI.Endpoints.Payments
{
    [Route("api/payment")]
    public class WompiWebhookEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult
    {
        private readonly IPaymentService _paymentService;
        private readonly INotificationService _notificationService;
        private readonly IConfiguration _configuration;

        public WompiWebhookEndpoint(IPaymentService paymentService, INotificationService notificationService, IConfiguration configuration)
        {
            _paymentService = paymentService;
            _notificationService = notificationService;
            _configuration = configuration;
        }

        [HttpPost("webhook")]
        [SwaggerOperation(Summary = "Webhook de Wompi para confirmar pagos", Tags = new[] { "Pagos" })]
        public override async Task<ActionResult> HandleAsync(CancellationToken ct = default)
        {
            Request.EnableBuffering();
            var body = await new StreamReader(Request.Body).ReadToEndAsync(ct);
            Request.Body.Position = 0;

            // Verificar firma de Wompi
            var signature = Request.Headers["X-Wompi-Signature"].FirstOrDefault();
            var eventsSecret = _configuration["WOMPI_EVENTS_SECRET"];
            if (!string.IsNullOrEmpty(eventsSecret) && !string.IsNullOrEmpty(signature))
            {
                var computed = ComputeHmacSha256(body, eventsSecret);
                if (!computed.Equals(signature, StringComparison.OrdinalIgnoreCase))
                    return Unauthorized();
            }

            using var doc = JsonDocument.Parse(body);
            var root = doc.RootElement;

            if (!root.TryGetProperty("data", out var data)) return Ok();
            if (!data.TryGetProperty("transaction", out var tx)) return Ok();

            var reference = tx.TryGetProperty("reference", out var refProp) ? refProp.GetString() : null;
            var txId = tx.TryGetProperty("id", out var idProp) ? idProp.GetString() : null;
            var status = tx.TryGetProperty("status", out var statusProp) ? statusProp.GetString() : null;

            if (reference == null || txId == null || status == null) return Ok();

            await _paymentService.UpdatePaymentStatusAsync(reference, txId, status);

            return Ok();
        }

        private static string ComputeHmacSha256(string data, string key)
        {
            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(key));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(data));
            return Convert.ToHexString(hash).ToLower();
        }
    }
}
