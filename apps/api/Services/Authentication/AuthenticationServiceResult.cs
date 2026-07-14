using ClinicFlow.Api.Contracts.Authentication;

namespace ClinicFlow.Api.Services.Authentication;

public sealed record AuthenticationServiceResult(
    AuthenticationResponse? Response,
    int? ErrorStatusCode,
    string? ErrorTitle,
    string? ErrorDetail
)
{
    public bool Succeeded =>
        Response is not null;

    public static AuthenticationServiceResult Success(
        AuthenticationResponse response
    )
    {
        return new AuthenticationServiceResult(
            response,
            null,
            null,
            null
        );
    }

    public static AuthenticationServiceResult Failure(
        int statusCode,
        string title,
        string detail
    )
    {
        return new AuthenticationServiceResult(
            null,
            statusCode,
            title,
            detail
        );
    }
}