using System.Security.Claims;
using System.Text;
using System.Text.Json.Serialization;
using ClinicFlow.Api.Authorization;
using ClinicFlow.Api.Configuration;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Infrastructure.Exceptions;
using ClinicFlow.Api.Models;
using ClinicFlow.Api.Services.Authentication;
using ClinicFlow.Api.Services.Authorization;
using ClinicFlow.Api.Services.Scheduling;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;

var builder =
    WebApplication.CreateBuilder(args);

builder.Services
    .AddControllers()
    .AddJsonOptions(options =>
    {
        options
            .JsonSerializerOptions
            .Converters
            .Add(
                new JsonStringEnumConverter()
            );
    });

builder.Services
    .AddEndpointsApiExplorer();

builder.Services
    .AddSwaggerGen(options =>
    {
        options.SwaggerDoc(
            "v1",
            new OpenApiInfo
            {
                Title = "ClinicFlow API",
                Version = "v1",
                Description =
                    "API para gestão de clínicas, médicos, pacientes e consultas."
            }
        );

        options.AddSecurityDefinition(
            "bearer",
            new OpenApiSecurityScheme
            {
                Type =
                    SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description =
                    "Informe somente o access token JWT."
            }
        );

        options.AddSecurityRequirement(
            document =>
                new OpenApiSecurityRequirement
                {
                    [
                        new OpenApiSecuritySchemeReference(
                            "bearer",
                            document
                        )
                    ] = []
                }
        );
    });

var connectionString =
    builder.Configuration
        .GetConnectionString(
            "DefaultConnection"
        )
    ?? throw new InvalidOperationException(
        "A connection string 'DefaultConnection' não foi configurada."
    );

builder.Services
    .AddDbContext<
        ClinicFlowDbContext
    >(
        options =>
        {
            options.UseNpgsql(
                connectionString
            );
        }
    );

builder.Services
    .AddDataProtection()
    .SetApplicationName(
        "ClinicFlow"
    );

builder.Services
    .AddIdentityCore<
        ApplicationUser
    >(
        options =>
        {
            options.User
                .RequireUniqueEmail =
                    true;

            options.Password
                .RequiredLength =
                    8;

            options.Password
                .RequireDigit =
                    true;

            options.Password
                .RequireLowercase =
                    true;

            options.Password
                .RequireUppercase =
                    true;

            options.Password
                .RequireNonAlphanumeric =
                    true;

            options.Password
                .RequiredUniqueChars =
                    4;

            options.Lockout
                .AllowedForNewUsers =
                    true;

            options.Lockout
                .MaxFailedAccessAttempts =
                    5;

            options.Lockout
                .DefaultLockoutTimeSpan =
                    TimeSpan
                        .FromMinutes(15);
        }
    )
    .AddRoles<
        IdentityRole<Guid>
    >()
    .AddEntityFrameworkStores<
        ClinicFlowDbContext
    >()
    .AddSignInManager()
    .AddDefaultTokenProviders();

builder.Services
    .Configure<
        AdminSeedOptions
    >(
        builder.Configuration
            .GetSection(
                AdminSeedOptions
                    .SectionName
            )
    );

var jwtSection =
    builder.Configuration
        .GetSection(
            JwtOptions.SectionName
        );

var jwtOptions =
    jwtSection
        .Get<JwtOptions>()
    ?? throw new InvalidOperationException(
        "As configurações do JWT não foram encontradas."
    );

if (
    string.IsNullOrWhiteSpace(
        jwtOptions.SecretKey
    )
    || Encoding.UTF8
        .GetByteCount(
            jwtOptions.SecretKey
        ) < 32
)
{
    throw new InvalidOperationException(
        "A chave secreta do JWT deve possuir pelo menos 32 bytes."
    );
}

if (
    string.IsNullOrWhiteSpace(
        jwtOptions.Issuer
    )
)
{
    throw new InvalidOperationException(
        "O emissor do JWT não foi configurado."
    );
}

if (
    string.IsNullOrWhiteSpace(
        jwtOptions.Audience
    )
)
{
    throw new InvalidOperationException(
        "O público do JWT não foi configurado."
    );
}

if (
    jwtOptions
        .AccessTokenExpirationMinutes
        <= 0
)
{
    throw new InvalidOperationException(
        "A expiração do access token deve ser maior que zero."
    );
}

if (
    jwtOptions
        .RefreshTokenExpirationDays
        <= 0
)
{
    throw new InvalidOperationException(
        "A expiração do refresh token deve ser maior que zero."
    );
}

builder.Services
    .Configure<JwtOptions>(
        jwtSection
    );

var signingKey =
    new SymmetricSecurityKey(
        Encoding.UTF8
            .GetBytes(
                jwtOptions.SecretKey
            )
    );

builder.Services
    .AddAuthentication(
        options =>
        {
            options
                .DefaultAuthenticateScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;

            options
                .DefaultChallengeScheme =
                    JwtBearerDefaults
                        .AuthenticationScheme;
        }
    )
    .AddJwtBearer(
        options =>
        {
            options.MapInboundClaims =
                false;

            options
                .TokenValidationParameters =
                    new TokenValidationParameters
                    {
                        ValidateIssuer =
                            true,

                        ValidIssuer =
                            jwtOptions
                                .Issuer,

                        ValidateAudience =
                            true,

                        ValidAudience =
                            jwtOptions
                                .Audience,

                        ValidateIssuerSigningKey =
                            true,

                        IssuerSigningKey =
                            signingKey,

                        ValidateLifetime =
                            true,

                        ClockSkew =
                            TimeSpan
                                .FromSeconds(30),

                        NameClaimType =
                            ClaimTypes.Name,

                        RoleClaimType =
                            ClaimTypes.Role
                    };
        }
    );

builder.Services
    .AddAuthorization(
        options =>
        {
            options.AddPolicy(
                AuthorizationPolicies
                    .ClinicUser,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser();

                    policy
                        .RequireRole(
                            UserRoleNames
                                .Admin,
                            UserRoleNames
                                .Doctor,
                            UserRoleNames
                                .Patient
                        );
                }
            );

            options.AddPolicy(
                AuthorizationPolicies
                    .AdminOnly,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser();

                    policy
                        .RequireRole(
                            UserRoleNames
                                .Admin
                        );
                }
            );

            options.AddPolicy(
                AuthorizationPolicies
                    .DoctorOnly,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser();

                    policy
                        .RequireRole(
                            UserRoleNames
                                .Doctor
                        );
                }
            );

            options.AddPolicy(
                AuthorizationPolicies
                    .PatientOnly,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser();

                    policy
                        .RequireRole(
                            UserRoleNames
                                .Patient
                        );
                }
            );

            options.AddPolicy(
                AuthorizationPolicies
                    .AdminOrDoctor,
                policy =>
                {
                    policy
                        .RequireAuthenticatedUser();

                    policy
                        .RequireRole(
                            UserRoleNames
                                .Admin,
                            UserRoleNames
                                .Doctor
                        );
                }
            );
        }
    );

builder.Services
    .AddScoped<
        AvailableSlotsService
    >();

builder.Services
    .AddHttpContextAccessor();

builder.Services
    .AddScoped<
        CurrentUserService
    >();

builder.Services
    .AddScoped<
        TokenService
    >();

builder.Services
    .AddScoped<
        AuthenticationService
    >();

builder.Services
    .AddProblemDetails();

builder.Services
    .AddExceptionHandler<
        GlobalExceptionHandler
    >();

// ==============================
// CORS
// PRECISA FICAR ANTES DO BUILD
// ==============================

var corsSettings =
    builder.Configuration
        .GetSection(
            CorsSettings.SectionName
        )
        .Get<CorsSettings>()
    ?? new CorsSettings();

if (
    corsSettings
        .AllowedOrigins
        .Length == 0
)
{
    throw new InvalidOperationException(
        "Nenhuma origem CORS foi configurada."
    );
}

builder.Services
    .AddCors(
        options =>
        {
            options.AddPolicy(
                CorsPolicyNames.Web,
                policy =>
                {
                    policy
                        .WithOrigins(
                            corsSettings
                                .AllowedOrigins
                        )
                        .AllowAnyHeader()
                        .AllowAnyMethod();
                }
            );
        }
    );


var app =
    builder.Build();

app.UseExceptionHandler();

await AdminSeeder.SeedAsync(
    app.Services
);

if (
    app.Environment
        .IsDevelopment()
)
{
    app.UseSwagger();

    app.UseSwaggerUI(
        options =>
        {
            options.SwaggerEndpoint(
                "/swagger/v1/swagger.json",
                "ClinicFlow API v1"
            );
        }
    );
}

app.UseCors(
    CorsPolicyNames.Web
);

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();