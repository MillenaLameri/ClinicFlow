using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Patients;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/patients/me")]
[Authorize(
    Policy =
        AuthorizationPolicies.PatientOnly
)]
public sealed class PatientProfileController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public PatientProfileController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    [ProducesResponseType(
        typeof(PatientResponse),
        StatusCodes.Status200OK
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    public async Task<
        ActionResult<PatientResponse>
    > GetMe(
        CancellationToken cancellationToken
    )
    {
        if (
            _currentUser.PatientId
                is not Guid patientId
        )
        {
            return Forbid();
        }

        var patient =
            await _dbContext.Patients
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id
                            == patientId
                        && item.IsActive,
                    cancellationToken
                );

        if (patient is null)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status404NotFound,
                title:
                    "Paciente não encontrado.",
                detail:
                    "O perfil do paciente autenticado não foi encontrado."
            );
        }

        return Ok(
            PatientResponse.FromEntity(
                patient
            )
        );
    }
}