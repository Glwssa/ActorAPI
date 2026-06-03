using ActorApi.Api.Exceptions;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ActorApi.Api.ExceptionHandling;

public sealed class GlobalExceptionHandler(
    ILogger<GlobalExceptionHandler> logger) : IExceptionHandler
{
    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        var problemDetails = exception switch
        {
            ActorApiException actorApiException => new ProblemDetails
            {
                Status = actorApiException.StatusCode,
                Title = actorApiException.Title,
                Detail = actorApiException.Message,
                Instance = httpContext.Request.Path
            },

            BadHttpRequestException badHttpRequestException => new ProblemDetails
            {
                Status = badHttpRequestException.StatusCode,
                Title = "Bad request",
                Detail = badHttpRequestException.Message,
                Instance = httpContext.Request.Path
            },

            _ => new ProblemDetails
            {
                Status = StatusCodes.Status500InternalServerError,
                Title = "Unexpected server error",
                Detail = "An unexpected error occurred while processing the request.",
                Instance = httpContext.Request.Path
            }
        };

        problemDetails.Extensions["traceId"] = httpContext.TraceIdentifier;

        if (problemDetails.Status >= StatusCodes.Status500InternalServerError)
        {
            logger.LogError(exception, "Unhandled exception occurred.");
        }
        else
        {
            logger.LogWarning(exception, "Handled request exception occurred.");
        }

        httpContext.Response.StatusCode =
            problemDetails.Status ?? StatusCodes.Status500InternalServerError;

        await httpContext.Response.WriteAsJsonAsync(
            problemDetails,
            cancellationToken);

        return true;
    }
}