using Api.Exceptions;
using Api.Models.Dtos;
using Microsoft.AspNetCore.Diagnostics;

namespace Api.Middleware;

public static class ExceptionHandlerExtensions
{
    public static void UseCustomExceptionHandler(this IApplicationBuilder app)
    {
        app.UseExceptionHandler(exceptionHandlerApp =>
        {
            exceptionHandlerApp.Run(async context =>
            {
                var feature = context.Features.Get<IExceptionHandlerFeature>();
                var exception = feature?.Error;

                if (exception != null)
                {
                    var logger = context.RequestServices
                        .GetRequiredService<ILogger<Program>>();

                    logger.LogError(exception, "Unhandled exception occurred while processing request.");
                }

                var (statusCode, message) = exception switch
                {
                    DomainException domainEx =>
                    (
                        StatusCodes.Status400BadRequest,
                        domainEx.Message
                    ),

                    BadHttpRequestException badRequestEx when badRequestEx.InnerException is System.Text.Json.JsonException =>
                    (
                        StatusCodes.Status400BadRequest,
                        "Invalid JSON format."
                    ),

                    _ =>
                    (
                        StatusCodes.Status500InternalServerError,
                        "An unexpected error occurred."
                    )
                };

                context.Response.StatusCode = statusCode;
                await context.Response.WriteAsJsonAsync(ParseResponse<object>.Failure(message));
            });
        });
    }
}