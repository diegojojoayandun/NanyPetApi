using DataAccessLayer.Entities.Enums;

namespace DataAccessLayer.Entities.DTO.Review
{
    public class ReviewDto
    {
        public string Id { get; set; } = null!;
        public string AppointmentId { get; set; } = null!;
        public string ReviewerId { get; set; } = null!;
        public string ReviewedId { get; set; } = null!;
        public ReviewType Type { get; set; }
        public int Rating { get; set; }
        public string? Comment { get; set; }
        public DateTime Created { get; set; }
    }
}
