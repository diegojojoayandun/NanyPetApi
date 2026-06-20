using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Pet
{
    public class PetUpdateDto
    {
        [MaxLength(60)]
        public string? Name { get; set; }

        [MaxLength(30)]
        public string? Species { get; set; }

        [MaxLength(30)]
        public string? Breed { get; set; }

        [Range(0, 50)]
        public int? Age { get; set; }

        [MaxLength(20)]
        public string? Gender { get; set; }
    }
}
