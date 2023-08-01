using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Login;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DataAccessLayer.Repositories.Users
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;
        private readonly UserManager<User> _userManager;
        private readonly IMapper _mapper;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly ILogger<UserRepository> _logger;


        public UserRepository(
            ApplicationDbContext context,
            IConfiguration configuration,
            UserManager<User> userManager,
            IMapper mapper,
            RoleManager<IdentityRole> roleManager,
            ILogger<UserRepository> logger)

        {
            _context = context;
            _configuration = configuration;
            _userManager = userManager;
            _mapper = mapper;
            _roleManager = roleManager;
            _logger = logger;

        }

        public bool IsUniqueUser(string userName)
        {
            try
            {
                var user = _context.Users.FirstOrDefault(u => u.Email.ToLower() == userName.ToLower());
                if (user == null)
                    return true;
            }
            catch (Exception ex)
            {
                ex.ToString();
            }

            return false;
        }

        public async Task<LoginResponseDTO> SignIn(LoginRequestDTO loginRequestDTO)
        {
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName.ToLower() == loginRequestDTO.UserName.ToLower());

            if (user == null)
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };

            bool isUserValid = await _userManager.CheckPasswordAsync(user, loginRequestDTO.Password);

            if (!isUserValid)
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };

            IList<string> roles = await _userManager.GetRolesAsync(user);

            if (roles == null)
            {
                return new LoginResponseDTO()
                {
                    Token = "",
                    User = null
                };
            }
            Console.WriteLine(user);
            var token = GenerateJwtToken(user.UserName, roles);
            LoginResponseDTO loginResponseDTO = new()
            {
                Token = token,
                User = _mapper.Map<UserDto>(user),
            };
            return loginResponseDTO;

        }

        public string GenerateJwtToken(string user, IList<string> roles)
        {
            // Header


            var _symmetricSecurityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["SECRET_KEY"]));
            var signingCredentials = new SigningCredentials(_symmetricSecurityKey, SecurityAlgorithms.HmacSha256);
            var header = new JwtHeader(signingCredentials);

            // Claims

            var claims = new List<Claim>();
            var role = roles.FirstOrDefault();

            if (user != null)
            {
                claims.Add(new Claim(ClaimTypes.Name, user));
            }

            if (!string.IsNullOrEmpty(role))
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            // Payload

            var payload = new JwtPayload
            (
                _configuration["Authentication:Issuer"],
                _configuration["Authentication:Audience"],
                claims,
                DateTime.Now,
                DateTime.UtcNow.AddMinutes(10)
            );
            var token = new JwtSecurityToken(header, payload);
            return new JwtSecurityTokenHandler().WriteToken(token);

        }

        public async Task<UserDto> SignUp(RegisterRequestDTO registerRequestDTO)
        {
            User user = new()
            {
                UserName = registerRequestDTO.Email,
                Email = registerRequestDTO.Email,
                NormalizedEmail = registerRequestDTO.Email.ToUpper(),
                FirstName = registerRequestDTO.FirstName,
                LastName = registerRequestDTO.LastName,
            };

            try
            {
                var result = await _userManager.CreateAsync(user, registerRequestDTO.Password);
                if (result.Succeeded)
                {
                    if (!_roleManager.RoleExistsAsync(registerRequestDTO.Rol).GetAwaiter().GetResult())
                    {
                        await _roleManager.CreateAsync(new IdentityRole(registerRequestDTO.Rol));
                    }
                    await _userManager.AddToRoleAsync(user, registerRequestDTO.Rol);
                    var userApp = _context.Users.FirstOrDefault(u => u.UserName == registerRequestDTO.Email);
                    return _mapper.Map<UserDto>(userApp);
                }
            }
            catch (Exception)
            {
                throw;
            }
            return new UserDto();
        }
    }
}
