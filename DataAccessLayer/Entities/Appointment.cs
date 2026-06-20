using DataAccessLayer.Entities.Common;
using DataAccessLayer.Entities.Enums;

namespace DataAccessLayer.Entities
{
    public partial class Appointment : AuditableBaseModel
    {
        public Appointment()
        {
            Messages = new HashSet<Message>();
            Reviews = new HashSet<Review>();
        }

        public string? PetId { get; set; }
        public string? HerderId { get; set; }

        public AppointmentStatus Status { get; set; } = AppointmentStatus.PendingRequest;
        public decimal Price { get; set; }
        public DateTime? AppointmentTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string? ServiceType { get; set; }
        public string? Notes { get; set; }
        public string? SpecialRequirements { get; set; }
        public string? CancellationReason { get; set; }

        // Referencia al pago
        public string? PaymentId { get; set; }
        public PaymentStatus? PaymentStatus { get; set; }

        public virtual Herder? Herder { get; set; }
        public virtual Pet? Pet { get; set; }
        public virtual Payment? Payment { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
        public virtual ICollection<Review> Reviews { get; set; }
    }
}
