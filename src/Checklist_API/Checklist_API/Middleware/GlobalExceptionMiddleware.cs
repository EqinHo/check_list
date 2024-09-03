using Checklist_API.Extensions;
using static Checklist_API.Extensions.CustomExceptions;

namespace Checklist_API.Middleware;
public class GlobalExceptionMiddleware : IMiddleware
{
    private readonly ILogger<GlobalExceptionMiddleware> _logger;
    private readonly ExceptionHandler _exceptionHandler;

    public GlobalExceptionMiddleware(ILogger<GlobalExceptionMiddleware> logger, ExceptionHandler exceptionHandler)
    {
        _logger = logger;
        _exceptionHandler = exceptionHandler;
    }

    public async Task InvokeAsync(HttpContext context, RequestDelegate next)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex)
        {
            await _exceptionHandler.HandleException (context, ex);
        }
    }
}