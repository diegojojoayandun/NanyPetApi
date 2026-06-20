using DataAccessLayer.Entities;

namespace BusinessLogicLayer.Services.PaymentService
{
    public interface IPaymentService
    {
        Task<Payment> CreatePaymentAsync(string appointmentId, decimal amount);
        Task<Payment?> GetPaymentByReferenceAsync(string wompiReference);
        Task UpdatePaymentStatusAsync(string wompiReference, string wompiTransactionId, string wompiStatus);
    }
}
