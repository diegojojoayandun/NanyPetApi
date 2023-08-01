using Ardalis.ApiEndpoints;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Mvc;

namespace NanyPetAPI.Endpoints.Owners
{
    public class ListAllOwnersEndpoint : EndpointBaseAsync
        .WithoutRequest
        .WithActionResult<APIResponse>
    {
        public override Task<ActionResult<APIResponse>> HandleAsync(CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
