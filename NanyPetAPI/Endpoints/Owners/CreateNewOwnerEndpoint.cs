using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.GenericService;
using BusinessLogicLayer.Services.OwnerService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Owner;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class CreateNewOwnerEndpoint : EndpointBaseAsync
        .WithRequest<OwnerCreateDto>
        .WithActionResult<APIResponse>
    {
        private readonly OwnerService _ownerService;

        public CreateNewOwnerEndpoint(OwnerService ownerService)

        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Create a Herder in the database
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crea un nuevo propietario en la Base de datos",
            Description = "Crea un nuevo propietario en la Base de datos",
            OperationId = "Create",
            Tags = new[] { "Propietarios" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(OwnerCreateDto request, CancellationToken cancellationToken = default)
        {
            APIResponse apiResponse = await _ownerService.Create(request, ModelState);

            return apiResponse.IsSuccess ? 
                CreatedAtRoute("GetOwner", new { id = apiResponse.Result }, apiResponse) : 
                BadRequest(apiResponse.Result);
        }
    }
}
