using Ardalis.ApiEndpoints;
using BusinessLogicLayer.Services.BlobService;
using DataAccessLayer.Data;
using DataAccessLayer.Entities;
using DataAccessLayer.Entities.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Swashbuckle.AspNetCore.Annotations;
using System.Net;
using System.Security.Claims;

namespace NanyPetAPI.Endpoints.Verification
{
    [Authorize(Roles = "Herder")]
    [Route("api/herder")]
    public class UploadVerificationDocsEndpoint : EndpointBaseAsync.WithRequest<string>.WithActionResult
    {
        private readonly ApplicationDbContext _context;
        private readonly IBlobService _blobService;

        public UploadVerificationDocsEndpoint(ApplicationDbContext context, IBlobService blobService)
        {
            _context = context;
            _blobService = blobService;
        }

        [HttpPost("{id}/verification")]
        [SwaggerOperation(Summary = "Subir documentos de verificación (foto + cédula)", Tags = new[] { "Verificación" })]
        public override async Task<ActionResult> HandleAsync([FromRoute] string id, CancellationToken ct = default)
        {
            var herder = await _context.Herders.FindAsync(new object[] { id }, ct);
            if (herder == null)
                return NotFound(new APIResponse { StatusCode = HttpStatusCode.NotFound, IsSuccess = false });

            var form = await Request.ReadFormAsync(ct);
            var photoFile = form.Files.GetFile("photo");
            var frontFile = form.Files.GetFile("idFront");
            var backFile = form.Files.GetFile("idBack");
            var selfieFile = form.Files.GetFile("selfieWithId");

            if (photoFile != null)
                herder.PhotoUrl = await _blobService.UploadFileAsync(photoFile.OpenReadStream(), photoFile.FileName, "profile-photos");

            if (frontFile != null)
                herder.IdDocumentFrontUrl = await _blobService.UploadFileAsync(frontFile.OpenReadStream(), frontFile.FileName, "verification-docs");

            if (backFile != null)
                herder.IdDocumentBackUrl = await _blobService.UploadFileAsync(backFile.OpenReadStream(), backFile.FileName, "verification-docs");

            if (selfieFile != null)
                herder.SelfieWithIdUrl = await _blobService.UploadFileAsync(selfieFile.OpenReadStream(), selfieFile.FileName, "verification-docs");

            herder.VerificationStatus = VerificationStatus.UnderReview;
            await _context.SaveChangesAsync(ct);

            return Ok(new APIResponse
            {
                StatusCode = HttpStatusCode.OK,
                Result = new { message = "Documentos enviados. Tu perfil está en revisión.", status = herder.VerificationStatus }
            });
        }
    }
}
