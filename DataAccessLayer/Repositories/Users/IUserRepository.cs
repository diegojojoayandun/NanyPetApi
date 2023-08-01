using DataAccessLayer.Entities.DTO.Login;

namespace DataAccessLayer.Repositories.Users
{
    public interface IUserRepository
    {
        bool IsUniqueUser(string userName);
        string GenerateJwtToken(string user, IList<string> roles);
        Task<LoginResponseDTO> SignIn(LoginRequestDTO loginRequestDTO);
        Task<UserDto> SignUp(RegisterRequestDTO registerRequestDTO);
    }
}
