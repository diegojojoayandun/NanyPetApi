using DataAccessLayer.Entities.Enums;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Review
{
    public class ReviewCreateDto
    {
        [Required]
        public string AppointmentId { get; set; } = null!;

        [Required]
        public ReviewType Type { get; set; }

        [Required]
        [Range(1, 5)]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
