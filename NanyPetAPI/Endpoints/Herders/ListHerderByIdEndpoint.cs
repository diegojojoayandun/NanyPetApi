﻿using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Services.GenericService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    [Route("api/herder")]
    public class ListHerderByIdEndpoint : EndpointBaseAsync
        .WithRequest<string>
        .WithActionResult<APIResponse>
    {
        private readonly IService<Herder> _herderService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ListHerderByIdEndpoint> _logger;
        protected APIResponse _apiResponse;
        public ListHerderByIdEndpoint(
            IService<Herder> herderService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<ListHerderByIdEndpoint> logger)
        {
            _herderService = herderService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Retrieves a specific Herder by unique id
        /// </summary>
        /// <param id="herder id">The herder id</param>
        /// <response code="200">herder retrieved</response>
        /// <response code="400">bad request</response>
        /// <response code="404">herder not found</response>
        [HttpGet("{id}", Name = "GetHerder")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Obtiene cuidador por Id",
            Description = "Obtiene cuidador por Id",
            OperationId = "GetHerderById",
            Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(string id, CancellationToken cancellationToken = default)
        {
            try
            {
                if (id == "")
                {
                    _logger.LogError("Error al buscar con Id " + id);
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    return BadRequest(_apiResponse);
                }

                var herder = await _herderService.GetById(v => v.Id == id);

                if (herder == null)
                {
                    _logger.LogError("No hay datos asociados a ese Id " + id);
                    _apiResponse.IsSuccess = false;
                    _apiResponse.StatusCode = HttpStatusCode.NotFound;
                    return NotFound(_apiResponse);
                }

                _apiResponse.Result = _mapper.Map<HerderDto>(herder);
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
