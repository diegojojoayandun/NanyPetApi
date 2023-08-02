

using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Services.UserService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Login;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Users
{
    public class SignupUsersEndpoint : EndpointBaseAsync
        .WithRequest<RegisterRequestDTO>
        .WithActionResult<LoginResponseDTO>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SignupUsersEndpoint> _logger;
        protected APIResponse _apiResponse;
        public SignupUsersEndpoint(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<SignupUsersEndpoint> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
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
        public override async Task<ActionResult<LoginResponseDTO>> HandleAsync(RegisterRequestDTO model, CancellationToken cancellationToken = default)
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
    }
}
