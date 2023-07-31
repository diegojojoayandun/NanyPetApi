using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Owner
{
    public class OwnerDto
    {
        public string Id { get; set; } = null!;
        [Required]
        [MaxLength(60)]
        [EmailAddress]
        public string? EmailUser { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
