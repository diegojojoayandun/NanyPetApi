using DataAccessLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DataAccessLayer.Entities
{
    public partial class Herder
    {
        public Herder()
        {
            Appointments = new HashSet<Appointment>();
            Messages = new HashSet<Message>();
        }

        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public string Id { get; set; } = null!;

        [Required]
        [EmailAddress]
        public string EmailUser { get; set; } = null!;

        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Location { get; set; }

        // Geolocalización
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? ServiceRadius { get; set; }

        // Tarifa y disponibilidad
        public decimal? HourlyRate { get; set; }
        public bool IsAvailable { get; set; } = true;

        // Verificación de identidad
        public VerificationStatus VerificationStatus { get; set; } = VerificationStatus.Pending;
        public string? PhotoUrl { get; set; }
        public string? IdDocumentFrontUrl { get; set; }
        public string? IdDocumentBackUrl { get; set; }
        public string? SelfieWithIdUrl { get; set; }
        public DateTime? VerifiedAt { get; set; }
        public string? VerifiedBy { get; set; }
        public string? RejectionReason { get; set; }

        // Rating calculado
        public double AverageRating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;

        // Token para notificaciones push
        public string? FcmToken { get; set; }

        public virtual User UserNameNavigation { get; set; } = null!;
        public virtual ICollection<Appointment> Appointments { get; set; }
        public virtual ICollection<Message> Messages { get; set; }
    }
}
