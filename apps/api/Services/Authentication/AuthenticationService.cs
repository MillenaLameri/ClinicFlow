using ClinicFlow.Api.Contracts.Authentication;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace ClinicFlow.Api.Services.Authentication;

public sealed class AuthenticationService
{
    private readonly ClinicFlowDbContext _dbContext;

    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly SignInManager<ApplicationUser>
        _signInManager;

    private readonly TokenService _tokenService;

    public AuthenticationService(
        ClinicFlowDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser>
            signInManager,
        TokenService tokenService
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _signInManager = signInManager;
        _tokenService = tokenService;
    }

    public async Task<AuthenticationServiceResult>
        RegisterPatientAsync(
            RegisterPatientRequest request,
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
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status400BadRequest,
                    "Dados de cadastro inválidos.",
                    exception.Message
                );
        }

        var cpfAlreadyExists =
            await _dbContext.Patients
                .AnyAsync(
                    item =>
                        item.Cpf == patient.Cpf,
                    cancellationToken
                );

        if (cpfAlreadyExists)
        {
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status409Conflict,
                    "CPF já cadastrado.",
                    "Já existe um paciente utilizando o CPF informado."
                );
        }

        var patientEmailAlreadyExists =
            await _dbContext.Patients
                .AnyAsync(
                    item =>
                        item.Email ==
                        patient.Email,
                    cancellationToken
                );

        if (patientEmailAlreadyExists)
        {
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status409Conflict,
                    "E-mail já cadastrado.",
                    "Já existe um paciente utilizando o e-mail informado."
                );
        }

        var userEmailAlreadyExists =
            await _userManager.FindByEmailAsync(
                patient.Email
            );

        if (userEmailAlreadyExists is not null)
        {
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status409Conflict,
                    "E-mail já cadastrado.",
                    "Já existe um usuário utilizando o e-mail informado."
                );
        }

        await using var transaction =
            await _dbContext.Database
                .BeginTransactionAsync(
                    cancellationToken
                );

        try
        {
            _dbContext.Patients.Add(patient);

            await _dbContext.SaveChangesAsync(
                cancellationToken
            );

            var user = new ApplicationUser(
                patient.FullName,
                patient.Email
            );

            user.LinkPatient(patient.Id);

            user.PhoneNumber = patient.Phone;

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

                return AuthenticationServiceResult
                    .Failure(
                        StatusCodes
                            .Status400BadRequest,
                        "Não foi possível criar o usuário.",
                        JoinIdentityErrors(
                            createUserResult
                        )
                    );
            }

            var addRoleResult =
                await _userManager.AddToRoleAsync(
                    user,
                    UserRoleNames.Patient
                );

            if (!addRoleResult.Succeeded)
            {
                await transaction.RollbackAsync(
                    cancellationToken
                );

                return AuthenticationServiceResult
                    .Failure(
                        StatusCodes
                            .Status500InternalServerError,
                        "Não foi possível definir o perfil do usuário.",
                        JoinIdentityErrors(
                            addRoleResult
                        )
                    );
            }

            var authenticationResponse =
                await _tokenService.CreateAsync(
                    user,
                    cancellationToken
                );

            await transaction.CommitAsync(
                cancellationToken
            );

            return AuthenticationServiceResult
                .Success(
                    authenticationResponse
                );
        }
        catch (DbUpdateException exception)
            when (
                IsUniqueConstraintViolation(
                    exception
                )
            )
        {
            await transaction.RollbackAsync(
                cancellationToken
            );

            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status409Conflict,
                    "Cadastro já existente.",
                    "Já existe um cadastro utilizando o CPF ou e-mail informado."
                );
        }
    }

    public async Task<AuthenticationServiceResult>
        LoginAsync(
            LoginRequest request,
            CancellationToken cancellationToken
        )
    {
        var normalizedEmail =
            request.Email
                .Trim()
                .ToLowerInvariant();

        var user =
            await _userManager.FindByEmailAsync(
                normalizedEmail
            );

        if (user is null)
        {
            return InvalidCredentials();
        }

        if (!user.IsActive)
        {
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status401Unauthorized,
                    "Usuário inativo.",
                    "Este usuário está inativo e não pode acessar o sistema."
                );
        }

        var signInResult =
            await _signInManager
                .CheckPasswordSignInAsync(
                    user,
                    request.Password,
                    lockoutOnFailure: true
                );

        if (signInResult.IsLockedOut)
        {
            return AuthenticationServiceResult
                .Failure(
                    423,
                    "Usuário temporariamente bloqueado.",
                    "O usuário foi bloqueado temporariamente após várias tentativas de login."
                );
        }

        if (!signInResult.Succeeded)
        {
            return InvalidCredentials();
        }

        var authenticationResponse =
            await _tokenService.CreateAsync(
                user,
                cancellationToken
            );

        return AuthenticationServiceResult
            .Success(authenticationResponse);
    }

    public async Task<AuthenticationServiceResult>
        RefreshAsync(
            RefreshTokenRequest request,
            CancellationToken cancellationToken
        )
    {
        var authenticationResponse =
            await _tokenService.RefreshAsync(
                request.RefreshToken,
                cancellationToken
            );

        if (authenticationResponse is null)
        {
            return AuthenticationServiceResult
                .Failure(
                    StatusCodes.Status401Unauthorized,
                    "Refresh token inválido.",
                    "O refresh token não existe, expirou ou já foi revogado."
                );
        }

        return AuthenticationServiceResult
            .Success(authenticationResponse);
    }

    public async Task RevokeAsync(
        RevokeTokenRequest request,
        CancellationToken cancellationToken
    )
    {
        await _tokenService.RevokeAsync(
            request.RefreshToken,
            cancellationToken
        );
    }

    public async Task<AuthenticatedUserResponse?>
        GetCurrentUserAsync(
            Guid userId
        )
    {
        var user =
            await _userManager.FindByIdAsync(
                userId.ToString()
            );

        if (
            user is null
            || !user.IsActive
        )
        {
            return null;
        }

        var roles =
            await _userManager.GetRolesAsync(
                user
            );

        return new AuthenticatedUserResponse(
            user.Id,
            user.FullName,
            user.Email ?? string.Empty,
            roles.ToArray(),
            user.PatientId,
            user.DoctorId
        );
    }

    private static AuthenticationServiceResult
        InvalidCredentials()
    {
        return AuthenticationServiceResult
            .Failure(
                StatusCodes.Status401Unauthorized,
                "Credenciais inválidas.",
                "O e-mail ou a senha informada está incorreto."
            );
    }

    private static string JoinIdentityErrors(
        IdentityResult result
    )
    {
        return string.Join(
            "; ",
            result.Errors.Select(
                error => error.Description
            )
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