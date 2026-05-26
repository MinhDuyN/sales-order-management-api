using OrderAPI.DTOs.Common;
using OrderAPI.Exceptions;

namespace OrderAPI.Middlewares
{
    public class ExceptionHandlingMiddlewares
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionHandlingMiddlewares> _logger;

        public ExceptionHandlingMiddlewares(RequestDelegate next, ILogger<ExceptionHandlingMiddlewares> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context, ILogger<ExceptionHandlingMiddlewares> logger)
        {
            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex,"Have log errors: {Message}",ex.Message);
                await HandleExceptionAsync(context, ex, logger);
            }
        }

        private static async Task HandleExceptionAsync(HttpContext context, Exception ex, ILogger logger)
        {
            context.Response.ContentType = "application/json";

            var statusCode = ex switch
            {
                NotFoundException => StatusCodes.Status404NotFound,
                BadRequestException => StatusCodes.Status400BadRequest,
                ConflictDataException => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };
            if (statusCode == StatusCodes.Status500InternalServerError)
            {
                logger.LogError(ex, "An error server: {Message}", ex.Message);
            }
            else
            {
                logger.LogWarning(ex, "An handle exception: {StatusCode} and message:{Message}", statusCode, ex.Message);
            }
            context.Response.StatusCode = statusCode;

            var response = new ErrorResponse
            {
                Success = false,
                Message = statusCode == StatusCodes.Status500InternalServerError
                    //? "Internal server error"
                    ? $"{ex.Message} | Inner: {ex.InnerException?.Message}"
                    : ex.Message,
                Errors = null
            };

            await context.Response.WriteAsJsonAsync(response);
        }
    }
}
