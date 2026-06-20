using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Pet
{
    public class PetCreateDto
    {
        [Required]
        [MaxLength(60)]
        public string Name { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Species { get; set; } = null!;

        [Required]
        [MaxLength(30)]
        public string Breed { get; set; } = null!;

        [Required]
        [Range(0, 50)]
        public int Age { get; set; }

        [Required]
        [MaxLength(20)]
        public string Gender { get; set; } = null!;
    }
}
