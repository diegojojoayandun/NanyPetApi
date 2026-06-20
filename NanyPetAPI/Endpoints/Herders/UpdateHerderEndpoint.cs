using Ardalis.ApiEndpoints;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    public class HerderUpdateRequest
    {
        [FromRoute(Name = "id")] public string Id { get; set; } = null!;
        [FromBody] public HerderUpdateDto Body { get; set; } = null!;
    }

    [Authorize(Roles = "Herder,Admin")]
    [Route("api/herder")]
    public class UpdateHerderEndpoint : EndpointBaseAsync.WithRequest<HerderUpdateRequest>.WithActionResult
    {
        private readonly ApplicationDbContext _context;

        public UpdateHerderEndpoint(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPut("{id}")]
        [SwaggerOperation(Summary = "Actualizar datos de cuidador", Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult> HandleAsync(HerderUpdateRequest request, CancellationToken ct = default)
        {
            var herder = await _context.Herders.FindAsync(new object[] { request.Id }, ct);
            if (herder == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            if (request.Body.Phone != null) herder.Phone = request.Body.Phone;
            if (request.Body.Address != null) herder.Address = request.Body.Address;
            if (request.Body.City != null) herder.City = request.Body.City;
            if (request.Body.State != null) herder.State = request.Body.State;
            if (request.Body.Location != null) herder.Location = request.Body.Location;
            if (request.Body.Latitude.HasValue) herder.Latitude = request.Body.Latitude;
            if (request.Body.Longitude.HasValue) herder.Longitude = request.Body.Longitude;
            if (request.Body.ServiceRadius.HasValue) herder.ServiceRadius = request.Body.ServiceRadius;
            if (request.Body.HourlyRate.HasValue) herder.HourlyRate = request.Body.HourlyRate;
            if (request.Body.IsAvailable.HasValue) herder.IsAvailable = request.Body.IsAvailable.Value;

            await _context.SaveChangesAsync(ct);
            return Ok(new APIResponse { StatusCode = HttpStatusCode.OK });
        }
    }
}
