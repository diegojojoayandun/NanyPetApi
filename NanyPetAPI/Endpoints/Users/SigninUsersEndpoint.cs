

using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Login;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Users
{
    public class SigninUsersEndpoint : EndpointBaseAsync
        .WithRequest<LoginRequestDTO>
        .WithActionResult<LoginResponseDTO>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<SigninUsersEndpoint> _logger;
        protected APIResponse _apiResponse;
        public SigninUsersEndpoint(
            IUserService userService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<SigninUsersEndpoint> logger)
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
        public override async Task<ActionResult<LoginResponseDTO>> HandleAsync(LoginRequestDTO model, CancellationToken cancellationToken = default)
        {
            var loginResponse = await _userService.SignIn(model);
            if (loginResponse.User == null || string.IsNullOrEmpty(loginResponse.Token))
            {
                _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages.Add("Usuario o Password son Incorrectos!");
                return BadRequest(_apiResponse);
            }
            _apiResponse.StatusCode = HttpStatusCode.OK;
            _apiResponse.Result = loginResponse;
            _logger.LogInformation("Login Exitoso!");
            return Ok(_apiResponse);
        }

       
    }
}
