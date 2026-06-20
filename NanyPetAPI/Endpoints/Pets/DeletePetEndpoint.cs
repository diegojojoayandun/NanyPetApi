using Ardalis.ApiEndpoints;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Pets
{
    [Authorize(Roles = "Owner")]
    [Route("api/pet")]
    public class DeletePetEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public DeletePetEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpDelete("{id}")]
        [SwaggerOperation(Summary = "Eliminar mascota", Tags = new[] { "Mascotas" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var pet = await _context.Pets.FindAsync(new object[] { id }, ct);
            if (pet == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            _context.Pets.Remove(pet);
            await _context.SaveChangesAsync(ct);
            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK });
        }
    }
}
