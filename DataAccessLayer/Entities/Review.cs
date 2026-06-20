using DataAccessLayer.Entities.Common;
using DataAccessLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities
{
    public partial class Review : AuditableBaseModel
    {
        [Required]
        public string AppointmentId { get; set; } = null!;

        [Required]
        public string ReviewerId { get; set; } = null!;

        [Required]
        public string ReviewedId { get; set; } = null!;

        public ReviewType Type { get; set; }

        [Range(1, 5)]
        public int Rating { get; set; }

        public string? Comment { get; set; }

        public virtual Appointment Appointment { get; set; } = null!;
        public virtual User Reviewer { get; set; } = null!;
        public virtual User Reviewed { get; set; } = null!;
    }
}
