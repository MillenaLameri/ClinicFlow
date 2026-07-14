using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using ClinicFlow.Api.Configuration;
using ClinicFlow.Api.Contracts.Authentication;
using ClinicFlow.Api.Data;
using ClinicFlow.Api.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace ClinicFlow.Api.Services.Authentication;

public sealed class TokenService
{
    private readonly ClinicFlowDbContext _dbContext;

    private readonly UserManager<ApplicationUser>
        _userManager;

    private readonly JwtOptions _jwtOptions;

    public TokenService(
        ClinicFlowDbContext dbContext,
        UserManager<ApplicationUser> userManager,
        IOptions<JwtOptions> jwtOptions
    )
    {
        _dbContext = dbContext;
        _userManager = userManager;
        _jwtOptions = jwtOptions.Value;
    }

    public async Task<AuthenticationResponse>
        CreateAsync(
            ApplicationUser user,
            CancellationToken cancellationToken
        )
    {
        var now = DateTime.UtcNow;

        var accessTokenExpiresAtUtc =
            now.AddMinutes(
                _jwtOptions
                    .AccessTokenExpirationMinutes
            );

        var refreshTokenExpiresAtUtc =
            now.AddDays(
                _jwtOptions
                    .RefreshTokenExpirationDays
            );

        var rawRefreshToken =
            GenerateRefreshToken();

        var refreshTokenHash =
            HashRefreshToken(rawRefreshToken);

        var refreshToken = new RefreshToken(
            user.Id,
            refreshTokenHash,
            refreshTokenExpiresAtUtc
        );

        _dbContext.RefreshTokens.Add(
            refreshToken
        );

        var response =
            await CreateResponseAsync(
                user,
                rawRefreshToken,
                accessTokenExpiresAtUtc,
                refreshTokenExpiresAtUtc,
                now
            );

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return response;
    }

    public async Task<AuthenticationResponse?>
        RefreshAsync(
            string rawRefreshToken,
            CancellationToken cancellationToken
        )
    {
        if (
            string.IsNullOrWhiteSpace(
                rawRefreshToken
            )
        )
        {
            return null;
        }

        var tokenHash =
            HashRefreshToken(
                rawRefreshToken.Trim()
            );

        var storedRefreshToken =
            await _dbContext.RefreshTokens
                .Include(token => token.User)
                .SingleOrDefaultAsync(
                    token =>
                        token.TokenHash ==
                        tokenHash,
                    cancellationToken
                );

        if (
            storedRefreshToken is null
            || !storedRefreshToken.IsActive
            || !storedRefreshToken.User.IsActive
        )
        {
            return null;
        }

        var now = DateTime.UtcNow;

        var newRawRefreshToken =
            GenerateRefreshToken();

        var newRefreshTokenHash =
            HashRefreshToken(
                newRawRefreshToken
            );

        var accessTokenExpiresAtUtc =
            now.AddMinutes(
                _jwtOptions
                    .AccessTokenExpirationMinutes
            );

        var refreshTokenExpiresAtUtc =
            now.AddDays(
                _jwtOptions
                    .RefreshTokenExpirationDays
            );

        storedRefreshToken.Revoke(
            newRefreshTokenHash
        );

        var newRefreshToken =
            new RefreshToken(
                storedRefreshToken.UserId,
                newRefreshTokenHash,
                refreshTokenExpiresAtUtc
            );

        _dbContext.RefreshTokens.Add(
            newRefreshToken
        );

        var response =
            await CreateResponseAsync(
                storedRefreshToken.User,
                newRawRefreshToken,
                accessTokenExpiresAtUtc,
                refreshTokenExpiresAtUtc,
                now
            );

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );

        return response;
    }

    public async Task RevokeAsync(
        string rawRefreshToken,
        CancellationToken cancellationToken
    )
    {
        if (
            string.IsNullOrWhiteSpace(
                rawRefreshToken
            )
        )
        {
            return;
        }

        var tokenHash =
            HashRefreshToken(
                rawRefreshToken.Trim()
            );

        var storedRefreshToken =
            await _dbContext.RefreshTokens
                .SingleOrDefaultAsync(
                    token =>
                        token.TokenHash ==
                        tokenHash,
                    cancellationToken
                );

        if (
            storedRefreshToken is null
            || storedRefreshToken.IsRevoked
        )
        {
            return;
        }

        storedRefreshToken.Revoke();

        await _dbContext.SaveChangesAsync(
            cancellationToken
        );
    }

    private async Task<AuthenticationResponse>
        CreateResponseAsync(
            ApplicationUser user,
            string rawRefreshToken,
            DateTime accessTokenExpiresAtUtc,
            DateTime refreshTokenExpiresAtUtc,
            DateTime issuedAtUtc
        )
    {
        var roles =
            await _userManager.GetRolesAsync(
                user
            );

        var claims = new List<Claim>
        {
            new(
                JwtRegisteredClaimNames.Sub,
                user.Id.ToString()
            ),
            new(
                ClaimTypes.NameIdentifier,
                user.Id.ToString()
            ),
            new(
                ClaimTypes.Name,
                user.FullName
            ),
            new(
                JwtRegisteredClaimNames.Email,
                user.Email ?? string.Empty
            ),
            new(
                ClaimTypes.Email,
                user.Email ?? string.Empty
            ),
            new(
                JwtRegisteredClaimNames.Jti,
                Guid.NewGuid().ToString()
            ),
            new(
                JwtRegisteredClaimNames.Iat,
                new DateTimeOffset(
                    issuedAtUtc
                )
                .ToUnixTimeSeconds()
                .ToString(),
                ClaimValueTypes.Integer64
            )
        };

        foreach (var role in roles)
        {
            claims.Add(
                new Claim(
                    ClaimTypes.Role,
                    role
                )
            );
        }

        if (user.PatientId.HasValue)
        {
            claims.Add(
                new Claim(
                    "patient_id",
                    user.PatientId.Value.ToString()
                )
            );
        }

        if (user.DoctorId.HasValue)
        {
            claims.Add(
                new Claim(
                    "doctor_id",
                    user.DoctorId.Value.ToString()
                )
            );
        }

        var signingKey =
            new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(
                    _jwtOptions.SecretKey
                )
            );

        var signingCredentials =
            new SigningCredentials(
                signingKey,
                SecurityAlgorithms.HmacSha256
            );

        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: claims,
            notBefore: issuedAtUtc,
            expires: accessTokenExpiresAtUtc,
            signingCredentials:
                signingCredentials
        );

        var accessToken =
            new JwtSecurityTokenHandler()
                .WriteToken(jwt);

        var userResponse =
            new AuthenticatedUserResponse(
                user.Id,
                user.FullName,
                user.Email ?? string.Empty,
                roles.ToArray(),
                user.PatientId,
                user.DoctorId
            );

        return new AuthenticationResponse(
            accessToken,
            rawRefreshToken,
            accessTokenExpiresAtUtc,
            refreshTokenExpiresAtUtc,
            userResponse
        );
    }

    private static string
        GenerateRefreshToken()
    {
        var randomBytes =
            RandomNumberGenerator.GetBytes(64);

        return WebEncoders.Base64UrlEncode(
            randomBytes
        );
    }

    private static string HashRefreshToken(
        string rawRefreshToken
    )
    {
        var tokenBytes =
            Encoding.UTF8.GetBytes(
                rawRefreshToken
            );

        var hashBytes =
            SHA256.HashData(tokenBytes);

        return Convert.ToHexString(
            hashBytes
        );
    }
}