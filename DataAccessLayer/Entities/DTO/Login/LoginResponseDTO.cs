namespace DataAccessLayer.Entities.DTO.Login
{
    public class LoginResponseDTO
    {
        public UserDto? User { get; set; }
        public string? Token { get; set; } = null!;

        //public string Rol { get; set; }
    }
}
