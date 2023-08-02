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
    public class Query
    {
        [FromRoute(Name = "id")] public string id { get; set; } = null!;
        [FromBody] public HerderUpdateDto updateDto { get; set; } = null!;

    }
    [Route("api/herder")]
    public class UpdateHerderEndpoint : EndpointBaseAsync
        .WithRequest<Query>
        .WithActionResult
    {
        private readonly IService<Herder> _herderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<UpdateHerderEndpoint> _logger;
        protected APIResponse _apiResponse;
        public UpdateHerderEndpoint(
            IService<Herder> herderService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<UpdateHerderEndpoint> logger)
        {
            _herderService = herderService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Update a specific Herder by unique id
        /// </summary>
        /// <param id="herder Id">The herder id</param>
        /// <response code="200">herder retrieved</response>
        /// <response code="400">bad request</response>
        /// <response code="404">Product not found</response>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Actualiza datos de cuidador",
            Description = "Actualiza datos de cuidador",
            OperationId = "UpdateHerder",
            Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] Query request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (request.updateDto == null || request.id != request.updateDto.Id)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                Herder modelHerder = _mapper.Map<Herder>(request.updateDto);

                await _herderService.Update(modelHerder);
                _apiResponse.StatusCode = HttpStatusCode.NoContent;
                return Ok(_apiResponse);
            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }
            return BadRequest(_apiResponse);

        }
    }
}
