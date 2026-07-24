using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace VitalGest.Api.Filters;

/// <summary>
/// Filter que valida automaticamente o ModelState antes de executar a action.
/// Retorna 400 Bad Request com os erros de validação se houver.
/// </summary>
public class ValidationFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState
                .Where(e => e.Value?.Errors.Count > 0)
                .SelectMany(e => e.Value!.Errors.Select(x => new
                {
                    Field = e.Key,
                    Message = x.ErrorMessage
                }))
                .ToList();

            context.Result = new BadRequestObjectResult(new
            {
                Success = false,
                ErrorCode = "VALIDATION_ERROR",
                Message = "Erro de validação. Verifique os dados enviados.",
                Errors = errors
            });
        }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
        // Não precisa fazer nada após a execução
    }
}