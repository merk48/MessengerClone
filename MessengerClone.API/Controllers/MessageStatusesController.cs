using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.MessageStatuses.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/messages/{messageId:int}/status")]
    [ApiController]
    public class MessageStatusesController(IMessageStatusService _messageStatusService, IUserContext _userContext) : ControllerBase
    {
        [HttpPost("acknowledge",Name = "UpdateMessageStatus")]
        public async Task<IActionResult> Acknowledge([FromRoute] int messageId, [FromBody] MessageAcknowledgeDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                Result result = new();

                if (dto.Status == enMessageStatus.Delivered)
                    result = await _messageStatusService.MarkAsDeliveredAsync(messageId, _userContext.UserId);

                else if (dto.Status == enMessageStatus.Read)
                    result = await _messageStatusService.MarkAsReadAsync(messageId, _userContext.UserId);

                return result.Succeeded
                   ? SuccessResponse("Message status updated successfully.")
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


        [HttpGet("unread", Name = "GetChatUnreadMessagesForUserAsync")]
        public async Task<IActionResult> GetChatUnreadMessagesForUserAsync([FromRoute] int chatId, [FromQuery] int? page = null, [FromQuery] int? size = null)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageStatusService.GetChatUnreadMessagesForUserAsync(chatId, _userContext.UserId, page, size);

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


        [HttpGet("unread/count", Name = "GetChatUnreadMessagesForUserCountAsync")]
        public async Task<IActionResult> GetChatUnreadMessagesForUserCountAsync([FromRoute] int chatId)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageStatusService.GetChatUnreadMessagesForUserCountAsync(chatId, _userContext.UserId);

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

    }
}
