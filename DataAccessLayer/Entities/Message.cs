using DataAccessLayer.Entities.Common;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public partial class Message : AuditableBaseModel
    {
        [Required]
        public string AppointmentId { get; set; } = null!;

        [Required]
        public string SenderId { get; set; } = null!;

        [Required]
        public string Content { get; set; } = null!;

        public DateTime SentAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;

        public virtual Appointment Appointment { get; set; } = null!;
        public virtual User Sender { get; set; } = null!;
    }
}
