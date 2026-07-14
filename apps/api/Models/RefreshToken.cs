using System.ComponentModel.DataAnnotations.Schema;

namespace ClinicFlow.Api.Models;

public sealed class RefreshToken
{
    public Guid Id { get; private set; }

    public Guid UserId { get; private set; }

    public ApplicationUser User { get; private set; } =
        null!;

    public string TokenHash { get; private set; } =
        string.Empty;

    public DateTime ExpiresAtUtc { get; private set; }

    public DateTime CreatedAtUtc { get; private set; }

    public DateTime? RevokedAtUtc { get; private set; }

    public string? ReplacedByTokenHash
    {
        get;
        private set;
    }

    [NotMapped]
    public bool IsExpired =>
        DateTime.UtcNow >= ExpiresAtUtc;

    [NotMapped]
    public bool IsRevoked =>
        RevokedAtUtc.HasValue;

    [NotMapped]
    public bool IsActive =>
        !IsExpired && !IsRevoked;

    private RefreshToken()
    {
        // Construtor utilizado pelo Entity Framework.
    }

    public RefreshToken(
        Guid userId,
        string tokenHash,
        DateTime expiresAtUtc
    )
    {
        if (userId == Guid.Empty)
        {
            throw new ArgumentException(
                "O usuário é obrigatório.",
                nameof(userId)
            );
        }

        if (string.IsNullOrWhiteSpace(tokenHash))
        {
            throw new ArgumentException(
                "O hash do refresh token é obrigatório.",
                nameof(tokenHash)
            );
        }

        if (expiresAtUtc <= DateTime.UtcNow)
        {
            throw new ArgumentException(
                "A expiração do refresh token deve estar no futuro.",
                nameof(expiresAtUtc)
            );
        }

        var normalizedHash = tokenHash.Trim();

        if (normalizedHash.Length > 128)
        {
            throw new ArgumentException(
                "O hash do refresh token é inválido.",
                nameof(tokenHash)
            );
        }

        Id = Guid.NewGuid();
        UserId = userId;
        TokenHash = normalizedHash;
        ExpiresAtUtc = expiresAtUtc;
        CreatedAtUtc = DateTime.UtcNow;
    }

    public void Revoke(
        string? replacedByTokenHash = null
    )
    {
        if (IsRevoked)
        {
            return;
        }

        RevokedAtUtc = DateTime.UtcNow;

        ReplacedByTokenHash =
            string.IsNullOrWhiteSpace(
                replacedByTokenHash
            )
                ? null
                : replacedByTokenHash.Trim();
    }
}