using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Owner
{
    public class OwnerCreateDto
    {
        [Required]
        [EmailAddress]
        [MaxLength(60)]
        public string? EmailUser { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
    }
}
