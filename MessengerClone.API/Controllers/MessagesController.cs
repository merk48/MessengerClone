using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Entities;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.DTOs;
using MessengerClone.Service.Features.Messages.DTOs;
using MessengerClone.Service.Features.Messages.Interfaces;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats/{chatId}/messages")]
    [ApiController]
    public class MessagesController(IUserContext _userContext ,IMessageService _messageService, IMessageStatusService _messageStatusService) : ControllerBase
    {

        [HttpGet(Name = "GetChatMessagesForUserAsync")]
        public async Task<IActionResult> GetChatMessagesForUserAsync([FromRoute] int chatId, CancellationToken cancellationToken, [FromQuery] int? page = null,[FromQuery] int? size = null, [FromQuery] string? search = null)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetChatMessagesForUserAsync(chatId, _userContext.UserId, cancellationToken, page, size, search);

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
        public async Task<IActionResult> GetMessageByIdForUserAsync([FromRoute] int chatId, [FromRoute] int Id, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetMessageByIdForUserAsync(Id, _userContext.UserId, cancellationToken);

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


        [HttpGet("latest", Name = "GetLatestMessageInChatAsync")]
        public async Task<IActionResult> GetLatestMessageInChatAsync([FromRoute] int chatId, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.GetLatestMessageInChatAsync(chatId, _userContext.UserId, cancellationToken);

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


        [HttpGet("unread", Name = "GetChatUnreadMessagesForUser")]
        public async Task<IActionResult> GetChatUnreadMessagesForUserAsync([FromRoute] int chatId, CancellationToken cancellationToken, [FromQuery] int? page = null, [FromQuery] int? size = null)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageStatusService.GetChatUnreadMessagesForUserAsync(chatId, _userContext.UserId, cancellationToken, page, size);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Chat unread messages retrieved successfully.")
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


        [HttpGet("unread-count", Name = "GetChatUnreadMessagesCountForUser")]
        public async Task<IActionResult> GetChatUnreadMessagesCountForUserAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageStatusService.GetChatUnreadMessagesCountForUserAsync(chatId, _userContext.UserId);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Chat unread messages number retrieved successfully.")
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
        public async Task<IActionResult> AddMessageAsync([FromRoute] int chatId, [FromForm]AddMessageDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.AddMessageAsync(dto, _userContext.UserId, chatId, cancellationToken);

                return result.Succeeded
                    ? CreatedResponse("GetMessageByIdForUserAsync", new { chatId = chatId, id = result.Data!.Id }, result.Data, "Message added successfully.")
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


        [HttpPatch("{id:int}/pin-toggle", Name = "PinOrUnpinMessageAsync")]
        public async Task<IActionResult> PinUnpinMessageAsync([FromRoute] int chatId, [FromRoute] int Id, [FromBody]PinUnPinMessageDto dto, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                Result<MessageDto> result = new();
                
                if(dto.Pin)
                    result = await _messageService.PinMessageAsync(Id, chatId, _userContext.UserId, cancellationToken);
                else
                    result = await _messageService.UnPinMessageAsync(Id, chatId, _userContext.UserId, cancellationToken);

                return result.Succeeded
                        ? SuccessResponse(result.Data, $"Message {(dto.Pin ? "pinned" : "unpinned")} successfully.")
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


        [HttpDelete("{id:int}", Name = "DeleteMessage")]
        public async Task<IActionResult> DeleteMessageAsync([FromRoute] int chatId, [FromRoute] int Id, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.DeleteMessageAsync(Id,chatId, _userContext.UserId, cancellationToken);

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


        [HttpPatch("{id:int}/undo-delete", Name = "UndoDeleteMessage")]
        public async Task<IActionResult> UndoDeleteMessageAsync([FromRoute] int chatId, [FromRoute] int Id, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageService.UndoDeleteMessageAsync(Id, chatId, _userContext.UserId, cancellationToken);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Deleted message restored successfully.")
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
