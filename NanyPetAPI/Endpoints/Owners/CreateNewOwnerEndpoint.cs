using Ardalis.ApiEndpoints;
using AutoMapper;
using BusinessLogicLayer.Services.GenericService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Owner;
using Microsoft.AspNetCore.Mvc;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;

namespace NanyPetAPI.Endpoints.Owners
{
    [Route("api/owner")]
    public class CreateNewOwnerEndpoint : EndpointBaseAsync
        .WithRequest<OwnerCreateDto>
        .WithActionResult<APIResponse>
    {
        private readonly IService<Owner> _ownerService;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly ILogger<CreateNewOwnerEndpoint> _logger;
        protected APIResponse _apiResponse;
        public CreateNewOwnerEndpoint(
            IService<Owner> ownerService,
            IMapper mapper,
            IConfiguration configuration,
            ILogger<CreateNewOwnerEndpoint> logger)
        {
            _ownerService = ownerService;
            _mapper = mapper;
            _configuration = configuration;
            _apiResponse = new APIResponse();
            _logger = logger;
        }

        /// <summary>
        /// Create a Herder in the database
        /// </summary>
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [SwaggerOperation(
            Summary = "Crea un nuevo cuidador en la Base de datos",
            Description = "Crea un nuevo cuidador en la Base de datos",
            OperationId = "CreateHerder",
            Tags = new[] { "Propietarios" })]
        public override async Task<ActionResult<APIResponse>> HandleAsync(OwnerCreateDto request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (await _ownerService.GetById(v => v.EmailUser == request.EmailUser) != null)
                {
                    ModelState.AddModelError("Error Usuario", "ya hay usuario asociado a ese email!");
                    return BadRequest(ModelState);
                }

                if (request == null)
                {
                    return BadRequest(request);
                }

                Owner modelHerder = _mapper.Map<Owner>(request);

                await _ownerService.Create(modelHerder);
                _apiResponse.Result = modelHerder;
                _apiResponse.StatusCode = HttpStatusCode.Created;
                return CreatedAtRoute("GetHerder", new { id = modelHerder.Id }, _apiResponse);

            }
            catch (Exception ex)
            {
                _apiResponse.IsSuccess = false;
                _apiResponse.ErrorMessages = new List<string> { ex.Message.ToString() };
            }

            return _apiResponse;

        }
    }
}
