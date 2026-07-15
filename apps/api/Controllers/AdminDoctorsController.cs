using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Admin;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/admin/doctors")]
[Authorize(
    Policy = AuthorizationPolicies.AdminOnly
)]
public sealed class AdminDoctorsController
    : ControllerBase
{
    private readonly ClinicFlowDbContext
        _dbContext;

    private readonly UserManager<ApplicationUser>
        _userManager;

    public AdminDoctorsController(
        ClinicFlowDbContext dbContext,
        UserManager<ApplicationUser> userManager
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
    }

    [HttpPost("{doctorId:guid}/account")]
    [ProducesResponseType(
        typeof(DoctorAccountResponse),
        StatusCodes.Status201Created
    )]
    [ProducesResponseType(
        StatusCodes.Status400BadRequest
    )]
    [ProducesResponseType(
        StatusCodes.Status404NotFound
    )]
    [ProducesResponseType(
        StatusCodes.Status409Conflict
    )]
    [ProducesResponseType(
        StatusCodes.Status401Unauthorized
    )]
    [ProducesResponseType(
        StatusCodes.Status403Forbidden
    )]
    public async Task<
        ActionResult<DoctorAccountResponse>
    > CreateAccount(
        Guid doctorId,
        [FromBody]
        CreateDoctorAccountRequest request,
        CancellationToken cancellationToken
    )
    {
        var doctor =
            await _dbContext.Doctors
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == doctorId,
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
                    $"Não foi encontrado um médico com o ID '{doctorId}'."
            );
        }

        if (!doctor.IsActive)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status400BadRequest,
                title:
                    "Médico inativo.",
                detail:
                    "Não é possível criar uma conta para um médico inativo."
            );
        }

        var doctorAlreadyHasAccount =
            await _dbContext.Users
                .AsNoTracking()
                .AnyAsync(
                    user =>
                        user.DoctorId
                            == doctorId,
                    cancellationToken
                );

        if (doctorAlreadyHasAccount)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status409Conflict,
                title:
                    "Médico já possui conta.",
                detail:
                    "Já existe uma conta de acesso vinculada a este médico."
            );
        }

        var existingUserByEmail =
            await _userManager
                .FindByEmailAsync(
                    doctor.Email
                );

        if (existingUserByEmail is not null)
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status409Conflict,
                title:
                    "E-mail já utilizado.",
                detail:
                    "Já existe uma conta de usuário utilizando o e-mail deste médico."
            );
        }

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        try
        {
            var user =
                new ApplicationUser(
                    doctor.FullName,
                    doctor.Email
                );

            user.LinkDoctor(
                doctor.Id
            );

            user.PhoneNumber =
                doctor.Phone;

            var createUserResult =
                await _userManager.CreateAsync(
                    user,
                    request.Password
                );

            if (!createUserResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken
                );

                return Problem(
                    statusCode:
                        StatusCodes
                            .Status400BadRequest,
                    title:
                        "Não foi possível criar a conta do médico.",
                    detail:
                        JoinIdentityErrors(
                            createUserResult
                        )
                );
            }

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    UserRoleNames.Doctor
                );

            if (!addRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken
                );

                return Problem(
                    statusCode:
                        StatusCodes
                            .Status500InternalServerError,
                    title:
                        "Não foi possível definir o perfil do médico.",
                    detail:
                        JoinIdentityErrors(
                            addRoleResult
                        )
                );
            }

            await transaction.CommitAsync(
                cancellationToken
            );

            var response =
                new DoctorAccountResponse(
                    user.Id,
                    doctor.Id,
                    doctor.FullName,
                    doctor.Email,
                    UserRoleNames.Doctor,
                    user.IsActive
                );

            return StatusCode(
                StatusCodes.Status201Created,
                response
            );
        }
        catch
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            throw;
        }
    }

    private static string JoinIdentityErrors(
        IdentityResult result
    )
    {
        return string.Join(
            "; ",
            result.Errors.Select(
                error =>
                    error.Description
            )
        );
    }
}