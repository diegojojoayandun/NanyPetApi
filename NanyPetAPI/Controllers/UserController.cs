using AutoMapper;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Login;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Controllers
{

    [Route("api")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UserController> _logger;
        protected APIResponse _apiResponse;

        public UserController(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<UserController> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Login for a registered user
        /// </summary>
        /// <response code=200>Successful login</response>
        /// <response code=400>Bad Request</response>
        /// <response code=500>Internal Server Error</response>
        [HttpPost("signin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Login for a registered user",
            Description = "Login for a registered user",
            OperationId = "signin",
            Tags = new[] { "Usuarios" })]
        public async Task<IActionResult> SignIn([FromBody] LoginRequestDTO model)
        {

            var loginResponse = await _userService.SignIn(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Usuario o Password son Icorrectos!");
                return BadRequest(_apiResponse);
            }
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = loginResponse;
            _logger.LogInformation("Login Exitoso!");
            return Ok(_apiResponse);
        }

        /// <summary>
        /// User Registration
        /// </summary>
        /// <response code=200>Successful login</response>
        /// <response code=400>Bad Request</response>
        /// <response code=500>Internal Server Error</response>
        [HttpPost("signup")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "User Registration",
            Description = "User Registration",
            OperationId = "signup",
            Tags = new[] { "Usuarios" })]
        public async Task<IActionResult> SignUp([FromBody] RegisterRequestDTO model)
        {
            bool isUniqueUser = _userService.IsUniqueUser(model.Email);

            if (!isUniqueUser)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Usuario ya existe!");
                return BadRequest(_apiResponse);
            }
            var user = await _userService.SignUp(model);

            if (user == null)
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Error al registrar usuario!");
                return BadRequest(_apiResponse);
            }
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.IsSuccess = true;
            return Ok(_apiResponse);
        }

        /// <summary>
        /// Google Login
        /// </summary>
        /// <response code=200>Successful login</response>
        /// <response code=400>Bad Request</response>
        /// <response code=500>Internal Server Error</response>
        [HttpGet("signin-google")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Google Login",
            Description = "Google Login",
            OperationId = "GoogleAuth",
            Tags = new[] { "Usuarios" })]
        public IActionResult GoogleAuth()
        {
            try
            {
                var authenticationProperties = new AuthenticationProperties
                {
                    RedirectUri = Url.Action(nameof(GoogleAuthCallback))
                };

                return Challenge(authenticationProperties, GoogleDefaults.AuthenticationScheme);
            }
            catch (Exception ex)
            {

                _logger.LogInformation(ex.Message.ToString());
                return BadRequest();
            }
        }

        /// <summary>
        /// Google Login
        /// </summary>
        /// <response code=200>Successful login</response>
        /// <response code=400>Bad Request</response>
        /// <response code=500>Internal Server Error</response>
        [HttpGet("handle-google-callback")]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [SwaggerOperation(
            Summary = "Google Login Callback",
            Description = "Google Login Callback returns token from user",
            OperationId = "GoogleAuthCallback",
            Tags = new[] { "Usuarios" })]
        public async Task<IActionResult> GoogleAuthCallback()
        {
            var authenticateResult = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            if (authenticateResult.Succeeded)
            {
                //// Authentication successful, handle the user's information.
                //var user = authenticateResult.Principal;

                //// Retrieve user's email
                //var userEmail = user.FindFirstValue(ClaimTypes.Email);

                //// Retrieve user's name
                //var userName = user.FindFirstValue(ClaimTypes.Name);

                //// Redirect or perform further actions.
                //// For example, you can return a success message or redirect to a specific URL.
                //return Ok(new { Email = userEmail, Name = userName });
                ////return RedirectToAction("GetAllUsers", "UserController", new { Email = userEmail, Name = userName });
                var user = authenticateResult.Principal;
                var userEmail = user.FindFirstValue(ClaimTypes.Email);

               //var token = _userRepository.GenerateJwtToken(userEmail);

                return Ok();
            }
            else
            {
                // Authentication failed, handle the error.
                return Unauthorized();
            }


        }

      

    }
}