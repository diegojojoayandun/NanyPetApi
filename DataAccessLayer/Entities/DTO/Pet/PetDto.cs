namespace DataAccessLayer.Entities.DTO.Pet
{
    public class PetDto
    {
        public string Id { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Species { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public int Age { get; set; }
        public string Gender { get; set; } = null!;
        public string? OwnerId { get; set; }
        public DateTime Created { get; set; }
    }
}
