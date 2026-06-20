using Ardalis.ApiEndpoints;
using BusinessLogicLayer.Services.PaymentService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Payments
{
    [Authorize(Roles = "Owner")]
    [Route("api/payment")]
    public class CreatePaymentEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IPaymentService _paymentService;

        public CreatePaymentEndpoint(ApplicationDbContext context, IPaymentService paymentService)
        {
            _context = context;
            _paymentService = paymentService;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Iniciar pago Wompi para una cita completada", Tags = new[] { "Pagos" })]
        public override async Task<ActionResult> HandleAsync([FromBody] string appointmentId, CancellationToken ct = default)
        {
            var appointment = await _context.Appointments.FindAsync(new object[] { appointmentId }, ct);
            if (appointment == null || appointment.Status != AppointmentStatus.Completed)
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "La cita debe estar completada para iniciar el pago." }
                });

            if (appointment.PaymentStatus == PaymentStatus.Approved)
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Esta cita ya fue pagada." }
                });

            var payment = await _paymentService.CreatePaymentAsync(appointmentId, appointment.Price);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = new { checkoutUrl = payment.WompiCheckoutUrl, reference = payment.WompiReference }
            });
        }
    }
}
