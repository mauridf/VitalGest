namespace VitalGest.Api.Middlewares;

/// <summary>
/// Middleware que garante um Correlation ID em todas as requisições.
/// Se o header X-Correlation-Id vier na requisição, usa ele.
/// Caso contrário, gera um novo GUID.
/// O ID é adicionado ao header da resposta e disponibilizado no HttpContext.
/// </summary>
public class CorrelationIdMiddleware
{
    private readonly RequestDelegate _next;
    private const string CorrelationIdHeader = "X-Correlation-Id";

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        // Verifica se o header já existe na requisição
        var correlationId = context.Request.Headers[CorrelationIdHeader].FirstOrDefault();

        // Se não existe, gera um novo
        if (string.IsNullOrEmpty(correlationId))
        {
            correlationId = Guid.NewGuid().ToString("N"); // Formato sem hífens
            context.Request.Headers[CorrelationIdHeader] = correlationId;
        }

        // Adiciona ao header da resposta
        context.Response.OnStarting(() =>
        {
            context.Response.Headers[CorrelationIdHeader] = correlationId;
            return Task.CompletedTask;
        });

        // Armazena no HttpContext.Items para uso em outras camadas
        context.Items["CorrelationId"] = correlationId;

        await _next(context);
    }
}

/// <summary>
/// Extensão para facilitar o registro do middleware.
/// </summary>
public static class CorrelationIdMiddlewareExtensions
{
    public static IApplicationBuilder UseCorrelationId(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<CorrelationIdMiddleware>();
    }
}