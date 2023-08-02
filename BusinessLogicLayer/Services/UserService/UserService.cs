using DataAccessLayer.Entities.DTO.Login;
using DataAccessLayer.Repositories.Users;

namespace BusinessLogicLayer.Services.UserService
{
    public class UserService : IUserService
    {
        private readonly IUserRepository _userRepository;
        public UserService(IUserRepository userRepository)
        {

            _userRepository = userRepository;
        }

        public bool IsUniqueUser(string userName)
        {
            return _userRepository.IsUniqueUser(userName);
        }

        public async Task<LoginResponseDTO> SignIn(LoginRequestDTO loginRequestDTO)
        {
            return await _userRepository.SignIn(loginRequestDTO);

        }

        public string GenerateJwtToken(string user, IList<string> roles)
        {
            return _userRepository.GenerateJwtToken(user, roles);
        }

        public Task<UserDto> SignUp(RegisterRequestDTO registerRequestDTO)
        {
            return _userRepository.SignUp(registerRequestDTO);
        }

    }
}
