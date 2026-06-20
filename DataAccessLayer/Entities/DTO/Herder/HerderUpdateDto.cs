namespace DataAccessLayer.Entities.DTO.Herder
{
    public class HerderUpdateDto
    {
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Location { get; set; }
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public double? ServiceRadius { get; set; }
        public decimal? HourlyRate { get; set; }
        public bool? IsAvailable { get; set; }
    }
}
