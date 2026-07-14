using System.ComponentModel.DataAnnotations;

namespace ClinicFlow.Api.Contracts.Authentication;

public sealed class LoginRequest
{
    [Required(
        ErrorMessage = "O e-mail é obrigatório."
    )]
    [EmailAddress(
        ErrorMessage = "O e-mail informado é inválido."
    )]
    public string Email { get; init; } =
        string.Empty;

    [Required(
        ErrorMessage = "A senha é obrigatória."
    )]
    public string Password { get; init; } =
        string.Empty;
}