using System.Security.Claims;
using VitalGest.Core.Interfaces;

namespace VitalGest.Api.Middlewares;

/// <summary>
/// Middleware que extrai o ClinicId do token JWT e define no TenantService.
/// Permite o funcionamento do Global Query Filter do EF Core.
/// </summary>
public class TenantResolutionMiddleware
{
    private readonly RequestDelegate _next;

    public TenantResolutionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context, ITenantService tenantService)
    {
        // Tenta extrair ClinicId do token JWT
        if (context.User.Identity?.IsAuthenticated == true)
        {
            var clinicIdClaim = context.User.FindFirst("clinic_id");

            if (clinicIdClaim != null && int.TryParse(clinicIdClaim.Value, out var clinicId))
            {
                tenantService.SetClinicId(clinicId);
            }
            else
            {
                // Usuário autenticado mas sem clinic_id (ex: SuperAdmin)
                tenantService.SetClinicId(null);
            }
        }
        else
        {
            // Requisição não autenticada (ex: login, health check)
            tenantService.SetClinicId(null);
        }

        await _next(context);
    }
}

public static class TenantResolutionMiddlewareExtensions
{
    public static IApplicationBuilder UseTenantResolution(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<TenantResolutionMiddleware>();
    }
}