using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Contracts.Patients;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Authorization;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Controllers;

[ApiController]
[Route("api/patients")]
[Authorize(
    Policy = AuthorizationPolicies.ClinicUser
)]
public sealed class PatientsController
    : ControllerBase
{
    private readonly ClinicFlowDbContext _dbContext;

    private readonly CurrentUserService
        _currentUser;

    public PatientsController(
        ClinicFlowDbContext dbContext,
        CurrentUserService currentUser
    )
    {
        _dbContext = dbContext;
        _currentUser = currentUser;
    }

    [HttpGet]
    public async Task<
        ActionResult<
            IReadOnlyCollection<PatientResponse>
        >
    > GetAll(
        [FromQuery] string? search = null,
        [FromQuery] bool includeInactive = false,
        CancellationToken cancellationToken = default
    )
    {
        var query = _dbContext.Patients
            .AsNoTracking()
            .AsQueryable();

        if (_currentUser.IsAdmin)
        {
            if (!includeInactive)
            {
                query = query.Where(
                    patient => patient.IsActive
                );
            }
        }
        else if (
            _currentUser.IsPatient
            && _currentUser.PatientId
                is Guid currentPatientId
        )
        {
            query = query.Where(
                patient =>
                    patient.Id == currentPatientId
                    && patient.IsActive
            );
        }
        else
        {
            return Forbid();
        }

        if (!string.IsNullOrWhiteSpace(search))
        {
            var normalizedSearch =
                search.Trim().ToLowerInvariant();

            var numericSearch = new string(
                search
                    .Where(char.IsDigit)
                    .ToArray()
            );

            query = query.Where(
                patient =>
                    patient.FullName
                        .ToLower()
                        .Contains(
                            normalizedSearch
                        )
                    || patient.Email
                        .ToLower()
                        .Contains(
                            normalizedSearch
                        )
                    || (
                        numericSearch
                            != string.Empty
                        && patient.Cpf.Contains(
                            numericSearch
                        )
                    )
            );
        }

        var patients = await query
            .OrderBy(
                patient =>
                    patient.FullName
            )
            .ToListAsync(
                cancellationToken
            );

        var response = patients
            .Select(
                PatientResponse.FromEntity
            )
            .ToList();

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<
        ActionResult<PatientResponse>
    > GetById(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        if (
            !_currentUser.IsAdmin
            && !(
                _currentUser.IsPatient
                && _currentUser.PatientId
                    == id
            )
        )
        {
            return Forbid();
        }

        var patient =
            await _dbContext.Patients
                .AsNoTracking()
                .SingleOrDefaultAsync(
                    item => item.Id == id,
                    cancellationToken
                );

        if (patient is null)
        {
            return PatientNotFound(id);
        }

        if (
            _currentUser.IsPatient
            && !patient.IsActive
        )
        {
            return PatientNotFound(id);
        }

        return Ok(
            PatientResponse.FromEntity(
                patient
            )
        );
    }

    [HttpPost]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    public async Task<
        ActionResult<PatientResponse>
    > Create(
        [FromBody]
        CreatePatientRequest request,
        CancellationToken cancellationToken
    )
    {
        Patient patient;

        try
        {
            patient = new Patient(
                request.FullName,
                request.Cpf,
                request.BirthDate,
                request.Email,
                request.Phone
            );
        }
        catch (ArgumentException exception)
        {
            return InvalidPatientData(
                exception.Message
            );
        }

        var cpfAlreadyExists =
            await _dbContext.Patients
                .AnyAsync(
                    item =>
                        item.Cpf
                        == patient.Cpf,
                    cancellationToken
                );

        if (cpfAlreadyExists)
        {
            return PatientCpfConflict(
                patient.Cpf
            );
        }

        var emailAlreadyExists =
            await _dbContext.Patients
                .AnyAsync(
                    item =>
                        item.Email
                        == patient.Email,
                    cancellationToken
                );

        if (emailAlreadyExists)
        {
            return PatientEmailConflict(
                patient.Email
            );
        }

        _dbContext.Patients.Add(
            patient
        );

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (
                IsUniqueConstraintViolation(
                    exception
                )
            )
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status409Conflict,
                title:
                    "Paciente já cadastrado.",
                detail:
                    "Já existe um paciente utilizando o CPF ou e-mail informado."
            );
        }

        var response =
            PatientResponse.FromEntity(
                patient
            );

        return CreatedAtAction(
            nameof(GetById),
            new
            {
                id = patient.Id
            },
            response
        );
    }

    [HttpPut("{id:guid}")]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    public async Task<
        ActionResult<PatientResponse>
    > Update(
        Guid id,
        [FromBody]
        UpdatePatientRequest request,
        CancellationToken cancellationToken
    )
    {
        var patient =
            await _dbContext.Patients
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id,
                    cancellationToken
                );

        if (patient is null)
        {
            return PatientNotFound(id);
        }

        var normalizedEmail =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var linkedUser =
            await _dbContext.Users
                .SingleOrDefaultAsync(
                    user =>
                        user.PatientId == id,
                    cancellationToken
                );

        // Por enquanto evitamos alterar o
        // e-mail de um paciente que já possui
        // conta de acesso, para não deixar
        // Patient e ApplicationUser divergentes.
        if (
            linkedUser is not null
            && patient.Email
                != normalizedEmail
        )
        {
            return Problem(
                statusCode:
                    StatusCodes
                        .Status409Conflict,
                title:
                    "E-mail vinculado a uma conta.",
                detail:
                    "O e-mail deste paciente está vinculado a uma conta de acesso e não pode ser alterado por este endpoint."
            );
        }

        var emailAlreadyExists =
            await _dbContext.Patients
                .AnyAsync(
                    item =>
                        item.Email
                            == normalizedEmail
                        && item.Id != id,
                    cancellationToken
                );

        if (emailAlreadyExists)
        {
            return PatientEmailConflict(
                normalizedEmail
            );
        }

        try
        {
            patient.UpdateProfile(
                request.FullName,
                request.BirthDate,
                request.Email,
                request.Phone
            );

            if (linkedUser is not null)
            {
                linkedUser.UpdateFullName(
                    patient.FullName
                );

                linkedUser.PhoneNumber =
                    patient.Phone;
            }
        }
        catch (ArgumentException exception)
        {
            return InvalidPatientData(
                exception.Message
            );
        }

        try
        {
            await _dbContext.SaveChangesAsync(
                cancellationToken
            );
        }
        catch (DbUpdateException exception)
            when (
                IsUniqueConstraintViolation(
                    exception
                )
            )
        {
            return PatientEmailConflict(
                normalizedEmail
            );
        }

        return Ok(
            PatientResponse.FromEntity(
                patient
            )
        );
    }

    [HttpDelete("{id:guid}")]
    [Authorize(
        Policy =
            AuthorizationPolicies.AdminOnly
    )]
    public async Task<IActionResult> Delete(
        Guid id,
        CancellationToken cancellationToken
    )
    {
        var patient =
            await _dbContext.Patients
                .SingleOrDefaultAsync(
                    item =>
                        item.Id == id,
                    cancellationToken
                );

        if (patient is null)
        {
            return PatientNotFound(id);
        }

        if (!patient.IsActive)
        {
            return NoContent();
        }

        patient.Deactivate();

        var linkedUser =
            await _dbContext.Users
                .SingleOrDefaultAsync(
                    user =>
                        user.PatientId == id,
                    cancellationToken
                );

        if (linkedUser is not null)
        {
            linkedUser.Deactivate();
        }

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return NoContent();
    }

    private ObjectResult PatientNotFound(
        Guid id
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status404NotFound,
            title:
                "Paciente não encontrado.",
            detail:
                $"Não foi encontrado um paciente com o ID '{id}'."
        );
    }

    private ObjectResult PatientCpfConflict(
        string cpf
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status409Conflict,
            title:
                "CPF já cadastrado.",
            detail:
                $"Já existe um paciente utilizando o CPF '{cpf}'."
        );
    }

    private ObjectResult PatientEmailConflict(
        string email
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status409Conflict,
            title:
                "E-mail já cadastrado.",
            detail:
                $"Já existe um paciente utilizando o e-mail '{email}'."
        );
    }

    private ObjectResult InvalidPatientData(
        string detail
    )
    {
        return Problem(
            statusCode:
                StatusCodes.Status400BadRequest,
            title:
                "Dados do paciente inválidos.",
            detail: detail
        );
    }

    private static bool
        IsUniqueConstraintViolation(
            DbUpdateException exception
        )
    {
        return exception.InnerException
            is PostgresException
            {
                SqlState:
                    PostgresErrorCodes
                        .UniqueViolation
            };
    }
}