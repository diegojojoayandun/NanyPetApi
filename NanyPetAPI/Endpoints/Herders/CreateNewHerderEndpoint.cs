using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.GenericServices;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    [Route("api/herder")]
    public class CreateNewHerderEndpoint : EndpointBaseAsync
        .WithRequest<HerderCreateDto>
        .WithActionResult<APIResponse>
    {
        private readonly IService<Herder> _herderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateNewHerderEndpoint> _logger;
        protected APIResponse _apiResponse;
        public CreateNewHerderEndpoint(
            IService<Herder> herderService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<CreateNewHerderEndpoint> logger)
        {
            _herderService = herderService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Create a Herder in the database
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crea un nuevo cuidador en la Base de datos",
            Description = "Crea un nuevo cuidador en la Base de datos",
            OperationId = "CreateHerder",
            Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(HerderCreateDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _herderService.GetById(v => v.EmailUser == request.EmailUser) != null)
                {
                    ModelState.AddModelError("Error Usuario", "ya hay usuario asociado a ese email!");
                    return BadRequest(ModelState);
                }

                if (request == null)
                {
                    return BadRequest(request);
                }

                Herder modelHerder = _mapper.Map<Herder>(request);

                await _herderService.Create(modelHerder);
                _apiResponse.Result = modelHerder;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetHerder", new { id = modelHerder.Id }, _apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message.ToString() };
            }

            return _apiResponse;

        }
    }
}
