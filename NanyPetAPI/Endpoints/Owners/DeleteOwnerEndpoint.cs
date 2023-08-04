using Ardalis.ApiEndpoints;
using Azure;
using BusinessLogicLayer.Services.OwnerService;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class DeleteOwnerEndpoint : EndpointBaseAsync
        .WithRequest<string>
        .WithActionResult
    {
        private readonly OwnerService _ownerService;

        public DeleteOwnerEndpoint(OwnerService ownerService)

        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Delete a specific Owner by unique id
        /// </summary>
        /// <param id="id">The herder id</param>
        /// <response code="200">herder retrieved</response>
        /// <response code="400">bad request</response>
        /// <response code="404">Product not found</response>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [SwaggerOperation(
            Summary = "Elimina los datos de propietario por su id",
            Description = "Elimina los datos de propietario por su id",
            OperationId = "Delete",
            Tags = new[] { "Propietarios" })]
        public override async Task<ActionResult> HandleAsync(string id, CancellationToken cancellationToken = default)
        {
            APIResponse apiResponse = await _ownerService.Delete(id);

            if (apiResponse.IsSuccess)
                return NoContent();
            else if (apiResponse.StatusCode == HttpStatusCode.NotFound)
                return NotFound(apiResponse);
            else
                return BadRequest(apiResponse);
        }
    }
}
