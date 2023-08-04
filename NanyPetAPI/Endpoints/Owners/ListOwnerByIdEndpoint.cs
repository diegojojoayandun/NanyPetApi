using Ardalis.ApiEndpoints;
using AutoMapper;
using Azure;
using BusinessLogicLayer.Services.GenericService;
using BusinessLogicLayer.Services.OwnerService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.DTO.Owner;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class ListOwnerByIdEndpoint : EndpointBaseAsync
        .WithRequest<string>
        .WithActionResult<APIResponse>
    {
        private readonly OwnerService _ownerService;

        public ListOwnerByIdEndpoint(OwnerService ownerService)
        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Retrieves a specific Herder by unique id
        /// </summary>
        /// <param id="herder id">The herder id</param>
        /// <response code="200">herder retrieved</response>
        /// <response code="400">bad request</response>
        /// <response code="404">herder not found</response>
        [HttpGet("{id}", Name = "GetOwner")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Obtiene cuidador por Id",
            Description = "Obtiene cuidador por Id",
            OperationId = "GetById",
            Tags = new[] { "Propietarios" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(string id, CancellationToken cancellationToken = default)
        {
            APIResponse apiResponse = await _ownerService.GetById(id);
            return Ok(apiResponse);
        }
    }
}
