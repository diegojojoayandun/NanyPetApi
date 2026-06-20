using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Pets
{
    public class UpdatePetRequest
    {
        [FromRoute] public string Id { get; set; } = null!;
        [FromBody] public PetUpdateDto Body { get; set; } = null!;
    }

    [Authorize(Roles = "Owner")]
    [Route("api/pet")]
    public class UpdatePetEndpoint : EndpointBaseAsync.WithRequest<UpdatePetRequest>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public UpdatePetEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Actualizar mascota", Tags = new[] { "Mascotas" })]
        public override async Task<ActionResult> HandleAsync(UpdatePetRequest request, CancellationToken ct = default)
        {
            var pet = await _context.Pets.FindAsync(new object[] { request.Id }, ct);
            if (pet == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            if (request.Body.Name != null) pet.Name = request.Body.Name;
            if (request.Body.Species != null) pet.Species = request.Body.Species;
            if (request.Body.Breed != null) pet.Breed = request.Body.Breed;
            if (request.Body.Age.HasValue) pet.Age = request.Body.Age.Value;
            if (request.Body.Gender != null) pet.Gender = request.Body.Gender;
            pet.LastModified = DateTime.UtcNow;

            await _context.SaveChangesAsync(ct);
            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = pet.Id });
        }
    }
}
