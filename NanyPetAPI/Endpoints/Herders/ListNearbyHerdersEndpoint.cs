using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.GeoService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Herder;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using NanyPet.Api.Models.Specifications;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Herders
{
    [Authorize]
    [Route("api/herder")]
    public class ListNearbyHerdersEndpoint : EndpointBaseAsync.WithoutRequest.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IMapper _mapper;
        private readonly IGeoService _geoService;

        public ListNearbyHerdersEndpoint(ApplicationDbContext context, IMapper mapper, IGeoService geoService)
        {
            _context = context;
            _mapper = mapper;
            _geoService = geoService;
        }

        [HttpGet("nearby")]
        [SwaggerOperation(Summary = "Buscar cuidadores verificados cercanos", Tags = new[] { "Cuidadores" })]
        public override async Task<ActionResult> HandleAsync(CancellationToken ct = default)
        {
            if (!double.TryParse(Request.Query["latitude"], out var lat) ||
                !double.TryParse(Request.Query["longitude"], out var lon))
                return BadRequest(new APIResponse
                {
                    StatusCode = HttpStatusCode.BadRequest,
                    IsSuccess = false,
                    ErrorMessages = new List<string> { "Se requieren los parámetros latitude y longitude." }
                });

            double.TryParse(Request.Query["radiusKm"], out var radiusKm);
            if (radiusKm <= 0) radiusKm = 10;

            int.TryParse(Request.Query["pageNumber"], out var pageNumber);
            if (pageNumber <= 0) pageNumber = 1;
            int.TryParse(Request.Query["pageSize"], out var pageSize);
            if (pageSize <= 0) pageSize = 20;

            // Pre-filtro por bounding box en SQL para reducir el dataset
            var latDelta = radiusKm / 111.0;
            var lonDelta = radiusKm / (111.0 * Math.Cos(lat * Math.PI / 180));

            var herders = await _context.Herders
                .Where(h => h.VerificationStatus == VerificationStatus.Verified
                         && h.IsAvailable
                         && h.Latitude.HasValue && h.Longitude.HasValue
                         && h.Latitude >= lat - latDelta && h.Latitude <= lat + latDelta
                         && h.Longitude >= lon - lonDelta && h.Longitude <= lon + lonDelta)
                .ToListAsync(ct);

            // Cálculo exacto Haversine en memoria
            var nearby = herders
                .Select(h => new
                {
                    Herder = h,
                    Distance = _geoService.CalculateDistance(lat, lon, h.Latitude!.Value, h.Longitude!.Value)
                })
                .Where(x => x.Distance <= radiusKm)
                .OrderBy(x => x.Distance)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            var result = nearby.Select(x =>
            {
                var dto = _mapper.Map<HerderDto>(x.Herder);
                dto.DistanceKm = Math.Round(x.Distance, 2);
                return dto;
            }).ToList();

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = result,
                TotalPages = 1
            });
        }
    }
}
