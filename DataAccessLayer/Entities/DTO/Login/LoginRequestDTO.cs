namespace DataAccessLayer.Entities.DTO.Login
{
    public class LoginRequestDTO
    {
        public string UserName { get; set; } = null!;
        public string Password { get; set; } = null!;
    }
}
