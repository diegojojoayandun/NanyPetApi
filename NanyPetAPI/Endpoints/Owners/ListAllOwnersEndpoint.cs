using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.DTO.Owner;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class ListAllOwnersEndpoint : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<APIResponse>
    {
        private readonly IService<Owner> _ownerService;
        private readonly ILogger<ListAllOwnersEndpoint> _logger;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        protected APIResponse _apiResponse;


        public ListAllOwnersEndpoint(
            IService<Owner> ownerService, 
            IConfiguration configuration, 
            IMapper mapper, 
            ILogger<ListAllOwnersEndpoint> logger)
        {
            _ownerService = ownerService;
            _apiResponse = new APIResponse();
            _configuration = configuration;
            _logger = logger;
            _mapper = mapper;

        }

        /// <summary>
        /// Retrieves a list with all Owners registered
        /// </summary>
        /// <response code="200">Herder's list retrieved</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Obtiene un listado de todos los propietarios registrados",
            Description = "Obtiene un listado de todos los propietarios registrados",
            OperationId = "GetAllOwners",
            Tags = new[] { "Propietarios" })]
        public  override async Task<ActionResult<APIResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de cuidadores"); // log -> show information on VS terminal
                IEnumerable<Owner> ownerList = await _ownerService.GetAll();
                _apiResponse.Result = _mapper.Map<IEnumerable<OwnerDto>>(ownerList);
                _apiResponse.StatusCode = HttpStatusCode.OK;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }

            return _apiResponse;

        }
    }
}
