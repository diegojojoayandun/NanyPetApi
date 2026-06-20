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
    [Authorize]
    [Route("api/pet")]
    public class ListAllPetsEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ListAllPetsEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Listar mascotas del owner autenticado", Tags = new[] { "Mascotas" })]
        public override async Task<ActionResult> HandleAsync(CancellationToken ct = default)
        {
            var userName = User.FindFirstValue(ClaimTypes.Name);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (user == null) return Unauthorized();

            var owner = await _context.Owners.FirstOrDefaultAsync(o => o.EmailUser == user.Email, ct);
            if (owner == null)
            {
                owner = new Owner { EmailUser = user.Email! };
                _context.Owners.Add(owner);
                await _context.SaveChangesAsync(ct);
            }

            var pets = await _context.Pets
                .Where(p => p.OwnerId == owner.Id)
                .ToListAsync(ct);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<List<PetDto>>(pets)
            });
        }
    }
}
