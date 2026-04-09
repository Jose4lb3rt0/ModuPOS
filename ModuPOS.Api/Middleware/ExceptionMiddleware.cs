using ModuPOS.Shared.DTOs;
using System.Net;
using System.Text.Json;

namespace ModuPOS.Api.Middleware
{
    public class ExceptionMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<ExceptionMiddleware> _logger;

        private static readonly JsonSerializerOptions JsonOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ExceptionMiddleware(RequestDelegate next, ILogger<ExceptionMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            try
            {
                await _next(context);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(
                "Error de negocio: {Message}", ex.Message);

                await EscribirRespuestaAsync(
                    context,
                    statusCode: (int)HttpStatusCode.BadRequest,
                    message: ex.Message,
                    type: "ValidationError");
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "Excepción no controlada en {Path}", context.Request.Path);

                await EscribirRespuestaAsync(
                    context,
                    statusCode: (int)HttpStatusCode.InternalServerError,
                    message: "Ocurrió un error interno. Por favor intente más tarde.",
                    type: "InternalError");
            }
        }

        private static async Task EscribirRespuestaAsync(
            HttpContext context, int statusCode, string message, string type)
            {
                if (context.Response.HasStarted)
                    return;

                var errorDetails = new ErrorDetails
                {
                    StatusCode = statusCode,
                    Message = message,
                    Type = type
                };

                context.Response.ContentType = "application/json";
                context.Response.StatusCode = statusCode;

                await context.Response.WriteAsync(
                    JsonSerializer.Serialize(errorDetails, JsonOptions));
            }
        }
    }


