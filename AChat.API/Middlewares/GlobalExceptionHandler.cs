using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using AChat.Domain.Exceptions;

namespace AChat.Middlewares;

public class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;
    private readonly IWebHostEnvironment _env;
    
    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger,
                                  IWebHostEnvironment env)
    {
        _logger = logger;
        _env = env;
    }
    
    public async ValueTask<bool> TryHandleAsync(HttpContext httpContext, 
                                                Exception exception, 
                                                CancellationToken cancellationToken)
    {
        _logger.LogError(exception, "Exception has occurred: {Message}", exception.Message);
        httpContext.Response.ContentType = "application/json";
        
        httpContext.Response.StatusCode = exception switch
        {
            AppException _ => (int) HttpStatusCode.BadRequest,
            AuthException _ => (int) HttpStatusCode.Unauthorized,
            NotFoundException _ => (int) HttpStatusCode.NotFound,
            AlreadyExistsException _ => (int) HttpStatusCode.BadRequest,
            _ => (int) HttpStatusCode.InternalServerError
        };
        
        var isDevelopment = _env.IsDevelopment();
        
        var problemDetails = new ProblemDetails
        {
            Status = httpContext.Response.StatusCode,
            Title = exception.Message,
            Detail = isDevelopment ? exception.StackTrace : null,
            Instance = httpContext.TraceIdentifier
        };

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
        
        return true;
    }
}
