using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Interfaces;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    public class DeleteHerderEndpoint : EndpointBaseAsync
        .WithRequest<string>
        .WithActionResult
    {

        private readonly IService<Herder> _herderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DeleteHerderEndpoint> _logger;
        protected APIResponse _apiResponse;
        public DeleteHerderEndpoint(
            IService<Herder> herderService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<DeleteHerderEndpoint> logger)
        {
            _herderService = herderService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Delete a specific Herder by unique id
        /// </summary>
        /// <param id="herder Id">The herder id</param>
        /// <response code="200">herder retrieved</response>
        /// <response code="400">bad request</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("api/herders/{id}", Name = "DeleteHerder")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Elimina los datos de cuidador por su id",
            Description = "Elimina los datos de cuidador por su id",
            OperationId = "DeleteHerder",
            Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult> HandleAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id == "")
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var herder = await _herderService.GetById(v => v.Id == id);

                if (herder == null)
                {
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }
                await _herderService.Delete(herder);

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
