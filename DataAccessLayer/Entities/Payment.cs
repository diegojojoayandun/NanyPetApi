using DataAccessLayer.Entities.Common;
using DataAccessLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public partial class Payment : AuditableBaseModel
    {
        [Required]
        public string AppointmentId { get; set; } = null!;

        [Required]
        public decimal Amount { get; set; }

        public string Currency { get; set; } = "COP";
        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string? WompiTransactionId { get; set; }
        public string? WompiReference { get; set; }
        public string? WompiCheckoutUrl { get; set; }

        public DateTime? UpdatedAt { get; set; }

        public virtual Appointment Appointment { get; set; } = null!;
    }
}
