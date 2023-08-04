using AutoMapper;
using BusinessLogicLayer.Services.GenericService;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.DTO.Owner;
using Microsoft.Extensions.Logging;
using System.Net;
using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace BusinessLogicLayer.Services.OwnerService
{
    public class OwnerService
    {
        private readonly IService<Owner> _IOwnerService;
        private readonly IMapper _mapper;
        private readonly ILogger<OwnerService> _logger;

        public OwnerService(
            IService<Owner> iOwnerService,
            IMapper mapper,
            ILogger<OwnerService> logger)
        {
            _IOwnerService = iOwnerService;
            _mapper = mapper;
            _logger = logger;
        }


        public async Task<APIResponse> GetAll()
        {
            var apiResponse = new APIResponse();

            try
            {
                _logger.LogInformation("Obteniendo lista de cuidadores");
                IEnumerable<Owner> ownerList = await _IOwnerService.GetAll();
                apiResponse.Result = _mapper.Map<IEnumerable<OwnerDto>>(ownerList);
                apiResponse.StatusCode = HttpStatusCode.OK;
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }

            return apiResponse;
        }

        public async Task<APIResponse> GetById(string id)
        {
            var apiResponse = new APIResponse();

            try
            {
                if (string.IsNullOrEmpty(id))
                {
                    _logger.LogError("Error al buscar con Id vacío o nulo");
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    var owner = await _IOwnerService.GetById(v => v.Id == id);

                    if (owner == null)
                    {
                        _logger.LogError("No hay datos asociados a ese Id " + id);
                        apiResponse.IsSuccess = false;
                        apiResponse.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        apiResponse.Result = _mapper.Map<OwnerDto>(owner);
                        apiResponse.StatusCode = HttpStatusCode.OK;
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages = new List<string> { ex.ToString() };
            }

            return apiResponse;
        }

        public async Task<APIResponse> Create(OwnerCreateDto request, ModelStateDictionary ModelState)
        {
            var apiResponse = new APIResponse();

            try
            {
                if (!ModelState.IsValid)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Result = ModelState;
                    return apiResponse;
                }

                if (await _IOwnerService.GetById(v => v.EmailUser == request.EmailUser) != null)
                {
                    ModelState.AddModelError("Error Usuario", "ya hay usuario asociado a ese email!");
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Result = ModelState;
                    return apiResponse;
                }

                if (request == null)
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                    apiResponse.Result = request;
                    return apiResponse;
                }

                Owner modelOwner = _mapper.Map<Owner>(request);

                await _IOwnerService.Create(modelOwner);
                apiResponse.Result = modelOwner;
                apiResponse.StatusCode = HttpStatusCode.Created;
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages = new List<string> { ex.Message.ToString() };
            }

            return apiResponse;
        }

        public async Task<APIResponse> Delete(string id)
        {
            var apiResponse = new APIResponse();

            try
            {
                if (id == "")
                {
                    apiResponse.IsSuccess = false;
                    apiResponse.StatusCode = HttpStatusCode.BadRequest;
                }
                else
                {
                    var owner = await _IOwnerService.GetById(v => v.Id == id);

                    if (owner == null)
                    {
                        apiResponse.IsSuccess = false;
                        apiResponse.StatusCode = HttpStatusCode.NotFound;
                    }
                    else
                    {
                        await _IOwnerService.Delete(owner);
                        apiResponse.StatusCode = HttpStatusCode.NoContent;
                    }
                }
            }
            catch (Exception ex)
            {
                apiResponse.IsSuccess = false;
                apiResponse.ErrorMessages = new List<string> { ex.Message };
            }

            return apiResponse;
        }
    }
}
