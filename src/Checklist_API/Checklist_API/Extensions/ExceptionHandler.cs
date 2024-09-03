using Microsoft.AspNetCore.Mvc;
using static Checklist_API.Extensions.CustomExceptions;

namespace Checklist_API.Extensions;

public class ExceptionHandler
{
    private readonly ILogger<ExceptionHandler> _logger;

    public ExceptionHandler(ILogger<ExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async Task HandleException(HttpContext context, Exception ex)
    {
        _logger.LogError(ex, "An error occurred - {@Machine} {@TraceId}",
            Environment.MachineName,
            System.Diagnostics.Activity.Current?.Id);

        var (statusCode, title) = ex switch
        {
            UserAlreadyExistsException => (StatusCodes.Status409Conflict, "User already exists"),
            _ => (StatusCodes.Status500InternalServerError, "An unexpected error occurred")
        };

        var problemDetails = new ProblemDetails
        {
            Title = title,
            Status = statusCode,
            Extensions = { ["traceId"] = System.Diagnostics.Activity.Current?.Id }
        };

        context.Response.StatusCode = statusCode;
        context.Response.ContentType = "application/json";

        await context.Response.WriteAsJsonAsync(problemDetails);
    }
}
