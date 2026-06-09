using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace VestaNET.Api.Middleware;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _log;
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> log) => _log = log;

    public async ValueTask<bool> TryHandleAsync(HttpContext ctx, Exception ex, CancellationToken ct)
    {
        var (status, title) = ex switch
        {
            KeyNotFoundException        => (404, "Não encontrado"),
            ArgumentException           => (400, "Requisição inválida"),
            UnauthorizedAccessException => (401, "Não autorizado"),
            _                           => (500, "Erro interno")
        };
        if (status == 500) _log.LogError(ex, "Erro não tratado");
        ctx.Response.StatusCode = status;
        await ctx.Response.WriteAsJsonAsync(new ProblemDetails
        {
            Status = status, Title = title,
            Detail = status == 500 ? "Ocorreu um erro inesperado." : ex.Message,
            Instance = ctx.Request.Path
        }, ct);
        return true;
    }
}
