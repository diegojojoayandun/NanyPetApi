using Ardalis.ApiEndpoints;
using AutoMapper;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Review;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Reviews
{
    [Authorize]
    [Route("api/review")]
    public class CreateReviewEndpoint : EndpointBaseAsync.WithRequest<ReviewCreateDto>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;

        public CreateReviewEndpoint(ApplicationDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Crear reseña tras cita completada", Tags = new[] { "Reseñas" })]
        public override async Task<ActionResult> HandleAsync(ReviewCreateDto request, CancellationToken ct = default)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var appointment = await _context.Appointments.FindAsync(new object[] { request.AppointmentId }, ct);
            if (appointment == null || appointment.Status != AppointmentStatus.Completed)
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Solo se pueden reseñar citas completadas." }
                });

            var userName = User.FindFirstValue(ClaimTypes.Name);
            var reviewer = await _context.Users.FirstOrDefaultAsync(u => u.UserName == userName, ct);
            if (reviewer == null) return Unauthorized();

            // Determinar el reviewedId según el tipo de reseña
            string reviewedId;
            if (request.Type == ReviewType.OwnerToHerder)
            {
                var herder = await _context.Herders.FindAsync(new object[] { appointment.HerderId! }, ct);
                var herderUser = await _context.Users.FirstOrDefaultAsync(u => u.UserName == herder!.EmailUser, ct);
                reviewedId = herderUser?.Id ?? "";
            }
            else
            {
                var pet = await _context.Pets.Include(p => p.Owner).FirstOrDefaultAsync(p => p.Id == appointment.PetId, ct);
                reviewedId = pet?.Owner?.UserNameNavigation?.Id ?? "";
            }

            if (string.IsNullOrEmpty(reviewedId))
                return BadRequest(new APIResponse { StatusCode = HttpStatusCode.BadRequest, IsSuccess = false });

            // Verificar que no haya reseña duplicada
            var existing = await _context.Reviews.AnyAsync(r =>
                r.AppointmentId == request.AppointmentId &&
                r.ReviewerId == reviewer.Id &&
                r.Type == request.Type, ct);

            if (existing)
                return Conflict(new APIResponse
                {
                    StatusCode = HttpStatusCode.Conflict,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Ya existe una reseña para esta cita." }
                });

            var review = new Review
            {
                AppointmentId = request.AppointmentId,
                ReviewerId = reviewer.Id,
                ReviewedId = reviewedId,
                Type = request.Type,
                Rating = request.Rating,
                Comment = request.Comment,
                Created = DateTime.UtcNow
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync(ct);

            // Actualizar rating promedio del herder si la reseña es hacia él
            if (request.Type == ReviewType.OwnerToHerder && appointment.HerderId != null)
            {
                var herder = await _context.Herders.FindAsync(new object[] { appointment.HerderId }, ct);
                if (herder != null)
                {
                    var ratings = await _context.Reviews
                        .Where(r => r.ReviewedId == reviewedId && r.Type == ReviewType.OwnerToHerder)
                        .Select(r => r.Rating)
                        .ToListAsync(ct);

                    herder.AverageRating = Math.Round(ratings.Average(), 2);
                    herder.TotalReviews = ratings.Count;
                    await _context.SaveChangesAsync(ct);
                }
            }

            return CreatedAtRoute("GetReview", new { id = review.Id }, new APIResponse
            {
                StatusCode = HttpStatusCode.Created,
                Result = _mapper.Map<ReviewDto>(review)
            });
        }
    }
}
