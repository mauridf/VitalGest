using System.Net;
using System.Text.Json;
using VitalGest.Core.Exceptions;

namespace VitalGest.Api.Middlewares;

/// <summary>
/// Middleware global de tratamento de exceções.
/// Captura todas as exceções não tratadas e retorna respostas padronizadas.
/// </summary>
public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
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
        catch (Exception ex)
        {
            await HandleExceptionAsync(context, ex);
        }
    }

    private async Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        var correlationId = context.Items["CorrelationId"]?.ToString() ?? "N/A";

        // Determina status code e mensagem baseado no tipo de exceção
        var (statusCode, errorCode, message) = exception switch
        {
            NotFoundException notFoundEx => (
                HttpStatusCode.NotFound,
                notFoundEx.ErrorCode,
                notFoundEx.Message
            ),
            BusinessRuleException businessEx => (
                HttpStatusCode.Conflict,
                businessEx.ErrorCode,
                businessEx.Message
            ),
            DomainException domainEx => (
                HttpStatusCode.UnprocessableEntity,
                domainEx.ErrorCode,
                domainEx.Message
            ),
            FluentValidation.ValidationException valEx => (
                HttpStatusCode.BadRequest,
                "VALIDATION_ERROR",
                "Erro de validação. Verifique os dados enviados."
            ),
            UnauthorizedAccessException => (
                HttpStatusCode.Unauthorized,
                "UNAUTHORIZED",
                "Acesso não autorizado."
            ),
            _ => (
                HttpStatusCode.InternalServerError,
                "INTERNAL_ERROR",
                "Ocorreu um erro interno no servidor."
            )
        };

        // Log apropriado baseado na severidade
        if (statusCode == HttpStatusCode.InternalServerError)
        {
            _logger.LogError(exception,
                "[CorrelationId: {CorrelationId}] Erro interno: {Message}",
                correlationId, exception.Message);
        }
        else
        {
            _logger.LogWarning(
                "[CorrelationId: {CorrelationId}] {ErrorCode}: {Message}",
                correlationId, errorCode, message);
        }

        // Monta resposta padronizada
        var response = new
        {
            Success = false,
            ErrorCode = errorCode,
            Message = message,
            CorrelationId = correlationId,
            // Inclui detalhes de validação se for ValidationException
            Errors = exception is FluentValidation.ValidationException valExWithErrors
                ? valExWithErrors.Errors.Select(e => new { e.PropertyName, e.ErrorMessage })
                : null,
            // Em desenvolvimento, inclui detalhes da exceção
            Details = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development"
                ? new { exception.StackTrace, InnerException = exception.InnerException?.Message }
                : null
        };

        context.Response.ContentType = "application/json";
        context.Response.StatusCode = (int)statusCode;

        var jsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response, jsonOptions));
    }
}

public static class GlobalExceptionMiddlewareExtensions
{
    public static IApplicationBuilder UseGlobalExceptionHandler(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<GlobalExceptionMiddleware>();
    }
}