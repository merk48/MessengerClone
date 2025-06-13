using Microsoft.AspNetCore.Mvc;

namespace MessengerClone.API.Response
{
    /// <summary>
    /// A helper class to create standardized API responses for various HTTP status codes.
    /// </summary>
    public static class ApiResponseHelper
    {
        /// <summary>
        /// Creates a successful HTTP 200 OK response with data and a message.
        /// </summary>
        /// <typeparam name="T">The type of the data object to include in the response.</typeparam>
        /// <param name="data">The data to be included in the response.</param>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 200 OK response.</returns>
        public static IActionResult SuccessResponse<T>(T data, string message = "Request successful")
        {
            return new ObjectResult(new ApiResponse<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = data,
                Message = message,
                Error = null
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        /// <summary>
        /// Creates a successful HTTP 200 OK response with a message and no data.
        /// </summary>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 200 OK response.</returns>
        public static IActionResult SuccessResponse(string message = "Operation completed successfully")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status200OK,
                Data = null,
                Message = message,
                Error = null
            })
            { StatusCode = StatusCodes.Status200OK };
        }

        /// <summary>
        /// Creates a HTTP 400 Bad Request response with error details.
        /// </summary>
        /// <param name="code">The unique error code identifying the failure.</param>
        /// <param name="message">The error message describing the failure.</param>
        /// <param name="details">Additional details about the error.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 400 Bad Request response.</returns>
        public static IActionResult BadRequestResponse(string code, string message, params string[] details)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status400BadRequest,
                Data = null,
                Message = message,
                Error = new ApiError(code, details)
            })
            { StatusCode = StatusCodes.Status400BadRequest };
        }

        /// <summary>
        /// Creates a HTTP 404 Not Found response with error details.
        /// </summary>
        /// <param name="code">The unique error code identifying the failure.</param>
        /// <param name="message">The error message describing the failure.</param>
        /// <param name="details">Additional details about the error.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 404 Not Found response.</returns>
        public static IActionResult NotFoundResponse(string code, string message, params string[] details)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status404NotFound,
                Data = null,
                Message = message,
                Error = new ApiError(code, details)
            })
            { StatusCode = StatusCodes.Status404NotFound };
        }

        /// <summary>
        /// Creates a HTTP 401 Unauthorized response with error details.
        /// </summary>
        /// <param name="code">The unique error code identifying the failure.</param>
        /// <param name="message">The error message describing the failure.</param>
        /// <param name="details">Additional details about the error.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 401 Unauthorized response.</returns>
        public static IActionResult UnauthorizedResponse(string code, string message, params string[] details)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = false,
                StatusCode = StatusCodes.Status401Unauthorized,
                Data = null,
                Message = message,
                Error = new ApiError(code, details)
            })
            { StatusCode = StatusCodes.Status401Unauthorized };
        }

        /// <summary>
        /// Creates a HTTP 204 No Content response with a success message.
        /// </summary>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 204 No Content response.</returns>
        public static IActionResult NoContentResponse(string message = "No content available")
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = true,
                StatusCode = StatusCodes.Status204NoContent,
                Data = null,
                Message = message,
                Error = null
            })
            { StatusCode = StatusCodes.Status204NoContent };
        }

        /// <summary>
        /// Creates a HTTP 201 Created response with the route and data details.
        /// </summary>
        /// <typeparam name="T">The type of the data object to include in the response.</typeparam>
        /// <param name="routeName">The name of the route used to retrieve the newly created resource.</param>
        /// <param name="routeValues">The values associated with the route.</param>
        /// <param name="data">The data of the newly created resource.</param>
        /// <param name="message">The success message to be included in the response.</param>
        /// <returns>An <see cref="IActionResult"/> representing the HTTP 201 Created response.</returns>
        public static IActionResult CreatedResponse<T>(string routeName, object routeValues, T data, string message = "Resource created successfully")
        {
            return new CreatedAtRouteResult(routeName, routeValues, new ApiResponse<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Data = data,
                Message = message,
                Error = null
            });
        }
        public static IActionResult CreatedResponse<T>(string routeName, object routeValues, string message = "Resource created successfully")
        {
            return new CreatedAtRouteResult(routeName, routeValues, new ApiResponse<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = message,
                Error = null
            });
        }
        public static IActionResult CreatedResponse<T>(object routeValues, string message = "Resource created successfully")
        {
            return new CreatedAtRouteResult(routeValues, new ApiResponse<T>
            {
                Success = true,
                StatusCode = StatusCodes.Status201Created,
                Message = message,
                Error = null
            });
        }

        /// <summary>
        /// Creates a response for any specified HTTP status code.
        /// </summary>
        /// <param name="statusCode">The HTTP status code for the response.</param>
        /// <param name="code">The unique error code identifying the response.</param>
        /// <param name="message">The message describing the response.</param>
        /// <param name="details">Additional details about the response.</param>
        /// <returns>An <see cref="IActionResult"/> representing the response for the specified HTTP status code.</returns>
        public static IActionResult StatusCodeResponse(int statusCode, string code, string message, params string[] details)
        {
            return new ObjectResult(new ApiResponse<object>
            {
                Success = statusCode is >= 200 and < 300, // Indicates success for status codes between 200 and 299
                StatusCode = statusCode,
                Data = null,
                Message = message,
                Error = statusCode is >= 400 ? new ApiError(code, details) : null
            })
            { StatusCode = statusCode };
        }
    }
}
