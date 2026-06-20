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

namespace NanyPetAPI.Endpoints.Pets
{
    [Authorize]
    [Route("api/pet")]
    public class ListPetByIdEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ListPetByIdEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("{id}", Name = "GetPet")]
        [SwaggerOperation(Summary = "Obtener mascota por ID", Tags = new[] { "Mascotas" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var pet = await _context.Pets.FindAsync(new object[] { id }, ct);
            if (pet == null)
                return NotFound(new APIResponse
                {
                    StatusCode = HttpStatusCode.NotFound,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Mascota no encontrada." }
                });

            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK, Result = _mapper.Map<PetDto>(pet) });
        }
    }
}
