using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Herder
{
    public class HerderCreateDto
    {

        [Required]
        public string? EmailUser { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }

    }
}
