using MessengerClone.Domain.Abstractions;
using MessengerClone.Domain.Utils.Enums;
using MessengerClone.Domain.Utils.Global;
using MessengerClone.Service.Features.MessageStatuses.DTOs;
using MessengerClone.Service.Features.MessageStatuses.Interfaces;
using Microsoft.AspNetCore.Mvc;
using static MessengerClone.API.Response.ApiResponseHelper;

namespace MessengerClone.API.Controllers
{
    [Route("api/chats/{chatId}/messages/{messageId:int}/status")]
    [ApiController]
    public class MessageStatusesController(IMessageStatusService _messageStatusService, IUserContext _userContext) : ControllerBase
    {
        [HttpGet(Name = "GetStatusesForMessage")]
        public async Task<IActionResult> GetStatusesForMessageAsync([FromRoute] int chatId, [FromRoute] int messageId, CancellationToken cancellationToken)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                var result = await _messageStatusService.GetStatusesForMessageAsync(chatId, messageId, _userContext.UserId, cancellationToken);

                return result.Succeeded
                    ? SuccessResponse(result.Data, $"Message statuses retrieved successfully.")
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


        [HttpPost("acknowledge", Name = "UpdateMessageStatus")]
        public async Task<IActionResult> Acknowledge([FromRoute] int chatId, [FromRoute] int messageId, [FromBody] MessageAcknowledgeDto dto)
        {
            try
            {
                if (_userContext.UserId <= 0)
                    return UnauthorizedResponse("INVALID_USER_ID", "User ID not valid.", "User should login first.");

                Result result = new();

                if (dto.Status == enMessageStatus.Delivered)
                    result = await _messageStatusService.MarkAsDeliveredAsync(chatId, messageId, _userContext.UserId);

                else if (dto.Status == enMessageStatus.Read)
                    result = await _messageStatusService.MarkAsReadAsync(chatId, messageId, _userContext.UserId);

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
    }
}
