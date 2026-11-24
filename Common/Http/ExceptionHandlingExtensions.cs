#nullable enable
using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using SemBackDotNet10.Common.exception;

namespace SemBackDotNet10.Common.exception.handler;

public static class ExceptionHandlingExtensions
{
    public static IServiceCollection AddCustomizedExceptionHandling(
        this IServiceCollection services,
        Action<CustomizedExceptionHandlerOptions>? configure = null)
    {
        services.AddLocalization();
        services.AddOptions<CustomizedExceptionHandlerOptions>();
        if (configure is not null) services.Configure(configure);
        services.AddExceptionHandler<CustomizedExceptionHandler>();
        services.AddProblemDetails();
        // MVC model validation occurs before the action and does not throw.
        services.Configure<ApiBehaviorOptions>(options =>
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var errors = context.ModelState.SelectMany(entry =>
                    entry.Value!.Errors.Select(error => entry.Key + ": " +
                        (string.IsNullOrWhiteSpace(error.ErrorMessage)
                            ? "Invalid value" : error.ErrorMessage))).ToList();
                return new BadRequestObjectResult(
                    new ErrorResponse("Data Validation Failed", errors));
            };
        });
        return services;
    }
}
