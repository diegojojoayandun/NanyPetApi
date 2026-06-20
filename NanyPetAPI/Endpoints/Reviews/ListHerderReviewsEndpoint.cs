using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Review;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Reviews
{
    [Route("api/review")]
    public class ListHerderReviewsEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public ListHerderReviewsEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpGet("herder/{herderId}", Name = "GetReview")]
        [SwaggerOperation(Summary = "Listar reseñas de un cuidador", Tags = new[] { "Reseñas" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string herderId, CancellationToken ct = default)
        {
            var herder = await _context.Herders.FindAsync(new object[] { herderId }, ct);
            if (herder == null) return NotFound();

            var herderUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == herder.EmailUser, ct);
            if (herderUser == null) return NotFound();

            var reviews = await _context.Reviews
                .Where(r => r.ReviewedId == herderUser.Id && r.Type == ReviewType.OwnerToHerder)
                .OrderByDescending(r => r.Created)
                .ToListAsync(ct);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = _mapper.Map<List<ReviewDto>>(reviews)
            });
        }
    }
}
