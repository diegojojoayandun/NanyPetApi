using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Appointment
{
    public class AppointmentCreateDto
    {
        [Required]
        public string PetId { get; set; } = null!;

        [Required]
        public string HerderId { get; set; } = null!;

        [Required]
        public DateTime AppointmentTime { get; set; }

        [Required]
        [Range(0.01, 9999999)]
        public decimal Price { get; set; }

        [MaxLength(50)]
        public string? ServiceType { get; set; }

        public string? Notes { get; set; }
        public string? SpecialRequirements { get; set; }
    }
}
