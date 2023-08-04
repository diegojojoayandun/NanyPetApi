using Ardalis.ApiEndpoints;
using BusinessLogicLayer.Services.OwnerService;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class ListAllOwnersEndpoint : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<APIResponse>
    {
        private readonly OwnerService _ownerService;

        public ListAllOwnersEndpoint(OwnerService ownerService) 
            
        {
            _ownerService = ownerService;
        }

        /// <summary>
        /// Retrieves a list with all Owners registered
        /// </summary>
        /// <response code="200">Owner's list retrieved</response>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [SwaggerOperation(
            Summary = "Obtiene un listado de todos los propietarios registrados",
            Description = "Obtiene un listado de todos los propietarios registrados",
            OperationId = "ListAllOwnersEndpoint",
            Tags = new[] { "Propietarios" })]
        public  override async Task<ActionResult<APIResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            APIResponse apiResponse = await _ownerService.GetAll();
            return Ok(apiResponse);
            
        }
    }
}
