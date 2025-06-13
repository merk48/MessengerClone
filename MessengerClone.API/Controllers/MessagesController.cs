using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.Messages.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [AllowAnonymous]
    [Route("api/chats/{chatId}/messages")]
    [ApiController]
    public class MessagesController(IUserContext _userContext ,IMessageService _messageService) : ControllerBase
    {

        [HttpGet(Name = "GetChatMessagesForUserAsync")]
        public async Task<IActionResult> GetChatMessagesForUserAsync([FromRoute] int chatId, [FromQuery] int? page = null,[FromQuery] int? size = null, [FromQuery] string? search = null)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetChatMessagesForUserAsync(chatId, _userContext.UserId, page, size, search);

                return result.Succeeded
                     ? SuccessResponse(result.Data, $"Chat messages retrieved successfully.")
                     : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpGet("{id:int}", Name = "GetMessageByIdForUserAsync")]
        public async Task<IActionResult> GetMessageByIdForUserAsync(int Id)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetMessageByIdForUserAsync(Id, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Message retrieved successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpGet("lastest", Name = "GetLastestMessageInChatAsync")]
        public async Task<IActionResult> GetLastestMessageInChatAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetLastestMessageInChatAsync(chatId, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Latest chat message retrieved successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "RETRIEVAL_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPost(Name = "AddMessageAsync")]
        public async Task<IActionResult> AddMessageAsync([FromRoute] int chatId, [FromForm]AddMessageDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.AddMessageAsync(dto,_userContext.UserId, chatId);

                return result.Succeeded
                    ? CreatedResponse("GetMessageByIdForUserAsync", new { id = result.Data!.Id }, result.Data, "Message added successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "CREATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch("{id:int}", Name = "PinOrUnpinMessageAsync")]
        public async Task<IActionResult> PinUnpinMessageAsync(int Id, [FromBody]PinUnPinMessageDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                Result<MessageDto> result = new();
                
                if(dto.IsPinned)
                    result = await _messageService.PinMessageAsync(Id, _userContext.UserId);
                else
                    result = await _messageService.UnPinMessageAsync(Id, _userContext.UserId);

                return result.Succeeded
                        ? SuccessResponse(result.Data, $"Message {(dto.IsPinned ? "pinned" : "unpinned")} successfully.")
                        : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpDelete( Name = "DeleteMessage")]
        public async Task<IActionResult> DeleteMessageAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.DeleteMessageAsync(chatId, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Message deleted successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "DELETION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }


        [HttpPatch(Name = "UndoDeleteMessage")]
        public async Task<IActionResult> UndoDeleteMessageAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.UndoDeleteMessageAsync(chatId, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Deleted message retrieved successfully.")
                    : StatusCodeResponse(StatusCodes.Status500InternalServerError, "ALTERATION_ERROR", result.ToString());
            }
            catch (HttpRequestException ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Service error: {ex.Message}");
            }
            catch (Exception ex)
            {
                //Log.Error(ex.Message);
                return StatusCodeResponse(StatusCodes.Status500InternalServerError, "ERROR_ACCRUED", "An error accrued", $"Unexpected error: {ex.Message}");
            }
        }

    }
}
