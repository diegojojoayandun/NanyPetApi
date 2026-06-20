using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Herder
{
    public class HerderCreateDto
    {
        [Required]
        [EmailAddress]
        public string EmailUser { get; set; } = null!;

        [MaxLength(30)]
        public string? Phone { get; set; }

        [MaxLength(100)]
        public string? Address { get; set; }

        [MaxLength(30)]
        public string? City { get; set; }

        [MaxLength(30)]
        public string? State { get; set; }

        [MaxLength(100)]
        public string? Location { get; set; }

        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? ServiceRadius { get; set; }

        [Range(0.01, 9999999)]
        public decimal? HourlyRate { get; set; }
    }
}
