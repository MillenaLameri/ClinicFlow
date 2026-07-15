using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Doctors;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/doctors/me")]
[Authorize(
    Policy =
        AuthorizationPolicies.DoctorOnly
)]
public sealed class DoctorProfileController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public DoctorProfileController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(DoctorResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<
        ActionResult<DoctorResponse>
    > GetMe(
        CancellationToken cancellationToken
    )
    {
        if (
            _currentUser.DoctorId
                is not Guid doctorId
        )
        {
            return Forbid();
        }

        var doctor =
            await _dbContext.Doctors
                .AsNoTracking()
                .Include(
                    item =>
                        item.Specialty
                )
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == doctorId
                        && item.IsActive,
                    cancellationToken
                );

        if (doctor is null)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status404NotFound,
                title:
                    "Médico não encontrado.",
                detail:
                    "O perfil do médico autenticado não foi encontrado."
            );
        }

        return Ok(
            DoctorResponse.FromEntity(
                doctor
            )
        );
    }
}