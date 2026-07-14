using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Authentication;

public sealed class RevokeTokenRequest
{
    [Required(
        ErrorMessage =
            "O refresh token é obrigatório."
    )]
    [MaxLength(
        500,
        ErrorMessage =
            "O refresh token informado é inválido."
    )]
    public string RefreshToken { get; init; } =
        string.Empty;
}