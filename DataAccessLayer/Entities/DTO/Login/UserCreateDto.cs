using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DataAccessLayer.Entities.DTO.Login
{
    public class UserCreateDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = null!;
        [Required]
        [PasswordPropertyText]
        public string Password { get; set; } = null!;
        public int RoleId { get; set; }
    }
}
