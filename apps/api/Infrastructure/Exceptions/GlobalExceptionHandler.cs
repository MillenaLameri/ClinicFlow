using System.Diagnostics;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Infrastructure.Exceptions;

public sealed class GlobalExceptionHandler
    : IExceptionHandler
{
    private readonly ILogger<
        GlobalExceptionHandler
    > _logger;

    public GlobalExceptionHandler(
        ILogger<GlobalExceptionHandler>
            logger
    )
    {
        _logger = logger;
    }

    public async ValueTask<bool>
        TryHandleAsync(
            HttpContext httpContext,
            Exception exception,
            CancellationToken
                cancellationToken
        )
    {
        var traceId =
            Activity.Current?.Id
            ?? httpContext
                .TraceIdentifier;

        _logger.LogError(
            exception,
            "Erro não tratado na requisição. TraceId: {TraceId}",
            traceId
        );

        var problemDetails =
            new ProblemDetails
            {
                Status =
                    StatusCodes
                        .Status500InternalServerError,

                Title =
                    "Erro interno do servidor.",

                Detail =
                    "Ocorreu um erro inesperado ao processar a solicitação.",

                Instance =
                    httpContext
                        .Request
                        .Path
            };

        problemDetails.Extensions[
            "traceId"
        ] = traceId;

        httpContext.Response.StatusCode =
            StatusCodes
                .Status500InternalServerError;

        await httpContext.Response
            .WriteAsJsonAsync(
                problemDetails,
                cancellationToken
            );

        return true;
    }
}