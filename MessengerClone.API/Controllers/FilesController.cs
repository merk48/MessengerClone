using MessengerClone.Service.Features.Files.Interfaces;
using MessengerClone.Service.Features.Users.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/attachments")]
    [ApiController]
    public class FilesController(IFileService _fileService) : ControllerBase
    {
        //public class AddFileDto
        //{
        //    public IFormFile File { get; set; } = null!;
        //}

        //[HttpPost(Name = "AddAttachment")]
        //public async Task<IActionResult> AddAttachment([FromBody] AddFileDto dto)
        //{
        //    try
        //    {
        //        var result = await _fileService.SaveAsync(dto.UserId, dto.Type, dto.NewValue);

        //        return result.Succeeded
        //             ? SuccessResponse("Attachment saved successfully.")
        //             : StatusCodeResponse(StatusCodes.Status500InternalServerError, "CREATION_ERROR", result.ToString());
        //    }
        //    catch (HttpRequestException ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
        //    }
        //    catch (Exception ex)
        //    {
        //        //Log.Error(ex.Message);
        //        return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
        //    }
        //}

    }
}
