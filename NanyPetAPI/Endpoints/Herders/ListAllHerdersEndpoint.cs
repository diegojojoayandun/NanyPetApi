using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.GenericService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    [Route("api/herder")]
    public class ListAllHerdersEndpoint : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<APIResponse>
    {
        private readonly IService<Herder> _herderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ListAllHerdersEndpoint> _logger;
        protected APIResponse _apiResponse;
        public ListAllHerdersEndpoint(
            IService<Herder> herderService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<ListAllHerdersEndpoint> logger)
        {
            _herderService = herderService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Retieves a List with all registered herders
        /// </summary>
        /// <response code="200">Herder's list retrieved</response>
        [HttpGet]
        [ResponseCache(Duration = 60)]
        //[Authorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [SwaggerOperation(
            Summary = "Obtiene un listado de todos los cuidadores registrados",
            Description = "Obtiene un listado de todos los cuidadores registrados",
            OperationId = "GetAllHerders",
            Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Obteniendo lista de cuidadores"); // log -> show information on VS terminal
                IEnumerable<Herder> herderList = await _herderService.GetAll();
                _apiResponse.Result = _mapper.Map<IEnumerable<HerderDto>>(herderList);
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
