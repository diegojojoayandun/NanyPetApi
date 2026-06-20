using DataAccessLayer.Entities.Enums;

namespace DataAccessLayer.Entities.DTO.Herder
{
    public class HerderDto
    {
        public string Id { get; set; } = null!;
        public string EmailUser { get; set; } = null!;
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? ServiceRadius { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool IsAvailable { get; set; }
        public VerificationStatus VerificationStatus { get; set; }
        public string? PhotoUrl { get; set; }
        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }
        public double? DistanceKm { get; set; }
    }
}
