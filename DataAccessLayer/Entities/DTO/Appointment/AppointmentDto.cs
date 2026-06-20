using DataAccessLayer.Entities.Enums;

namespace DataAccessLayer.Entities.DTO.Appointment
{
    public class AppointmentDto
    {
        public string Id { get; set; } = null!;
        public string? PetId { get; set; }
        public string? HerderId { get; set; }
        public AppointmentStatus Status { get; set; }
        public decimal Price { get; set; }
        public DateTime? AppointmentTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }
        public string? SpecialRequirements { get; set; }
        public string? CancellationReason { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }
        public DateTime Created { get; set; }
    }
}
