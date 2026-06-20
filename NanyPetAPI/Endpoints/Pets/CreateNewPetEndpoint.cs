using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Pet;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Pets
{
    [Authorize(Roles = "Owner")]
    [Route("api/pet")]
    public class CreateNewPetEndpoint : EndpointBaseAsync.WithRequest<PetCreateDto>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateNewPetEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Registrar nueva mascota", Tags = new[] { "Mascotas" })]
        public override async Task<ActionResult> HandleAsync(PetCreateDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (user == null) return Unauthorized();

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.EmailUser == user.Email, ct);
            if (owner == null)
            {
                // Auto-create the Owner profile if it's missing (handles accounts created
                // before the Owners table existed or where registration rolled back partially).
                owner = new Owner { EmailUser = user.Email! };
                _context.Owners.Add(owner);
                await _context.SaveChangesAsync(ct);
            }

            var pet = _mapper.Map<Pet>(request);
            pet.OwnerId = owner.Id;
            pet.Created = DateTime.UtcNow;

            _context.Pets.Add(pet);
            await _context.SaveChangesAsync(ct);

            return CreatedAtRoute("GetPet", new { id = pet.Id }, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Result = _mapper.Map<PetDto>(pet)
            });
        }
    }
}
