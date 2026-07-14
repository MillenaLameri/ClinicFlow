using System.Security.Claims;
using ClinicFlow.Api.Contracts.Authentication;
using ClinicFlow.Api.Services.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/auth")]
public sealed class AuthController : ControllerBase
{
    private readonly AuthenticationService
        _authenticationService;

    public AuthController(
        AuthenticationService
            authenticationService
    )
    {
        _authenticationService =
            authenticationService;
    }

    [AllowAnonymous]
    [HttpPost("register/patient")]
    [ProducesResponseType(
        typeof(AuthenticationResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status409Conflict
    )]
    public async Task<
        ActionResult<AuthenticationResponse>
    > RegisterPatient(
        [FromBody]
        RegisterPatientRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _authenticationService
                .RegisterPatientAsync(
                    request,
                    cancellationToken
                );

        if (!result.Succeeded)
        {
            return AuthenticationFailure(
                result
            );
        }

        return StatusCode(
            StatusCodes.Status201Created,
            result.Response
        );
    }

    [AllowAnonymous]
    [HttpPost("login")]
    [ProducesResponseType(
        typeof(AuthenticationResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized
    )]
    public async Task<
        ActionResult<AuthenticationResponse>
    > Login(
        [FromBody] LoginRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _authenticationService
                .LoginAsync(
                    request,
                    cancellationToken
                );

        if (!result.Succeeded)
        {
            return AuthenticationFailure(
                result
            );
        }

        return Ok(result.Response);
    }

    [AllowAnonymous]
    [HttpPost("refresh")]
    [ProducesResponseType(
        typeof(AuthenticationResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized
    )]
    public async Task<
        ActionResult<AuthenticationResponse>
    > Refresh(
        [FromBody]
        RefreshTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        var result =
            await _authenticationService
                .RefreshAsync(
                    request,
                    cancellationToken
                );

        if (!result.Succeeded)
        {
            return AuthenticationFailure(
                result
            );
        }

        return Ok(result.Response);
    }

    [AllowAnonymous]
    [HttpPost("revoke")]
    [ProducesResponseType(
        StatusCodes.Status204NoContent
    )]
    public async Task<IActionResult> Revoke(
        [FromBody]
        RevokeTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        await _authenticationService
            .RevokeAsync(
                request,
                cancellationToken
            );

        return NoContent();
    }

    [Authorize]
    [HttpGet("me")]
    [ProducesResponseType(
        typeof(AuthenticatedUserResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized
    )]
    public async Task<
        ActionResult<AuthenticatedUserResponse>
    > Me()
    {
        var userIdValue =
            User.FindFirstValue(
                ClaimTypes.NameIdentifier
            );

        if (
            !Guid.TryParse(
                userIdValue,
                out var userId
            )
        )
        {
            return UnauthorizedProblem();
        }

        var currentUser =
            await _authenticationService
                .GetCurrentUserAsync(userId);

        if (currentUser is null)
        {
            return UnauthorizedProblem();
        }

        return Ok(currentUser);
    }

    private ObjectResult AuthenticationFailure(
        AuthenticationServiceResult result
    )
    {
        return Problem(
            statusCode:
                result.ErrorStatusCode
                ?? StatusCodes
                    .Status400BadRequest,
            title:
                result.ErrorTitle
                ?? "Falha na autenticação.",
            detail:
                result.ErrorDetail
                ?? "Não foi possível concluir a autenticação."
        );
    }

    private ObjectResult UnauthorizedProblem()
    {
        return Problem(
            statusCode:
                StatusCodes.Status401Unauthorized,
            title: "Usuário não autenticado.",
            detail:
                "Não foi possível identificar o usuário autenticado."
        );
    }
}