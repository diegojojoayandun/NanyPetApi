using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Login
{
    public class RegisterRequestDTO
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        [Required(ErrorMessage = "Email Address is requiered")]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required(ErrorMessage = "Password is requiered")]
        public string Password { get; set; } = null!;
        public string Rol { get; set; } = null!;
    }
}
