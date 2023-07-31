using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Owner
{
    public class OwnerUpdateDto
    {
        [Required]
        public string Id { get; set; } = null!;
        [Required]
        public string? EmailUser { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
