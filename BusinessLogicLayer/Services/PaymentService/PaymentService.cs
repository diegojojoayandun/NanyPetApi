using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace BusinessLogicLayer.Services.PaymentService
{
    public class PaymentService : IPaymentService
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;

        private const string WompiSandboxBase = "https://sandbox.wompi.co/v1";

        public PaymentService(ApplicationDbContext context, IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _configuration = configuration;
            _httpClient = httpClientFactory.CreateClient("Wompi");
        }

        public async Task<Payment> CreatePaymentAsync(string appointmentId, decimal amount)
        {
            var reference = $"NANYPET-{appointmentId[..8].ToUpper()}-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}";
            var publicKey = _configuration["WOMPI_PUBLIC_KEY"] ?? "";

            var checkoutUrl = $"https://checkout.wompi.co/p/?public-key={publicKey}" +
                              $"&currency=COP" +
                              $"&amount-in-cents={(long)(amount * 100)}" +
                              $"&reference={reference}" +
                              $"&redirect-url={_configuration["FRONTEND_URL"]}/payment/callback";

            var payment = new Payment
            {
                AppointmentId = appointmentId,
                Amount = amount,
                Currency = "COP",
                Status = PaymentStatus.Pending,
                WompiReference = reference,
                WompiCheckoutUrl = checkoutUrl
            };

            _context.Payments.Add(payment);
            await _context.SaveChangesAsync();

            // Actualizar referencia en la cita
            var appointment = await _context.Appointments.FindAsync(appointmentId);
            if (appointment != null)
            {
                appointment.PaymentId = payment.Id;
                appointment.PaymentStatus = PaymentStatus.Pending;
                await _context.SaveChangesAsync();
            }

            return payment;
        }

        public async Task<Payment?> GetPaymentByReferenceAsync(string wompiReference)
        {
            return await _context.Payments
                .FirstOrDefaultAsync(p => p.WompiReference == wompiReference);
        }

        public async Task UpdatePaymentStatusAsync(string wompiReference, string wompiTransactionId, string wompiStatus)
        {
            var payment = await GetPaymentByReferenceAsync(wompiReference);
            if (payment == null) return;

            payment.WompiTransactionId = wompiTransactionId;
            payment.Status = wompiStatus == "APPROVED" ? PaymentStatus.Approved
                           : wompiStatus == "DECLINED" ? PaymentStatus.Rejected
                           : PaymentStatus.Processing;
            payment.UpdatedAt = DateTime.UtcNow;

            var appointment = await _context.Appointments.FindAsync(payment.AppointmentId);
            if (appointment != null)
                appointment.PaymentStatus = payment.Status;

            await _context.SaveChangesAsync();
        }
    }
}
